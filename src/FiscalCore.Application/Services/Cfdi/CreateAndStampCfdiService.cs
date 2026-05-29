
using FiscalCore.Application.DTOs.Certificate;
using FiscalCore.Application.DTOs.Cfdis;
using FiscalCore.Application.DTOs.Common;
using FiscalCore.Application.Interfaces.Certificate;
using FiscalCore.Application.Interfaces.Cfdis;
using FiscalCore.Application.Interfaces.Message;
using FiscalCore.Application.Interfaces.Pac;
using FiscalCore.Domain.Interfaces.Stamping;
using FluentValidation;
using System.Text;
using System.Xml.Linq;

namespace FiscalCore.Application.Services.Cfdi;

public sealed class CreateAndStampCfdiService : ICreateAndStampCfdiService
{
    private readonly IMessagesProvider _messagesProvider;
    private readonly IValidator<CreateCfdiRequest> _validatorCreateCfdiRequest;
    private readonly ICfdiFiscalRulesValidatorService _cfdiFiscalRulesValidatorService;
    private readonly ICfdiTotalsValidatorService _cfdiTotalsValidatorService;
    private readonly ICfdiXmlBuilder _xmlBuilder;
    private readonly ICfdiValidateXmlStructure _cfdiValidateXmlStructure;
    private readonly ICertificateService _certificateService;
    private readonly ICfdiSealService _cfdiSealService;
    private readonly IOriginalStringGeneratorService _originalStringGeneratorService;
    private readonly ICfdiXsdValidator _xsdValidator;
    private readonly IStampBalanceRepository _stampBalanceRepository;
    private readonly IPacStampingService _pacStampingService;
    private readonly ICfdiPersistenceService _cfdiPersistenceService;

    public CreateAndStampCfdiService(
        IMessagesProvider messagesProvider,
        IValidator<CreateCfdiRequest> validatorCreateCfdiRequest,
        ICfdiFiscalRulesValidatorService cfdiFiscalRulesValidatorService,
        ICfdiTotalsValidatorService cfdiTotalsValidatorService,
        ICfdiXmlBuilder xmlBuilder,
        ICfdiValidateXmlStructure cfdiValidateXmlStructure,
        ICertificateService certificateService,
        ICfdiSealService cfdiSealService,
        IOriginalStringGeneratorService originalStringGeneratorService,
        ICfdiXsdValidator xsdValidator,
        IStampBalanceRepository stampBalanceRepository,
        IPacStampingService pacStampingService,
        ICfdiPersistenceService cfdiPersistenceService)
    {
        _messagesProvider = messagesProvider;
        _validatorCreateCfdiRequest = validatorCreateCfdiRequest;
        _cfdiFiscalRulesValidatorService = cfdiFiscalRulesValidatorService;
        _cfdiTotalsValidatorService = cfdiTotalsValidatorService;
        _xmlBuilder = xmlBuilder;
        _cfdiValidateXmlStructure = cfdiValidateXmlStructure;
        _certificateService = certificateService;
        _cfdiSealService = cfdiSealService;
        _originalStringGeneratorService = originalStringGeneratorService;
        _xsdValidator = xsdValidator;
        _stampBalanceRepository = stampBalanceRepository;
        _pacStampingService = pacStampingService;
        _cfdiPersistenceService = cfdiPersistenceService;
    }

    public async Task<ResponseDto> ExecuteAsync(CreateCfdiRequest request, CancellationToken ct)
    {
        try
        {
            // FASE 1 – VALIDACIONES INTERNAS
            var validationInternalResult = await ValidateInternalAsync(request, ct);
            if (!validationInternalResult.IsSuccess)
                return validationInternalResult;

            var xml = ((ResponseSuccessDto<XDocument>)validationInternalResult).Data;

            // FASE 2 - OBTENER CSD, INSERTAR CERTIFICADO Y NOCERTIFICADO
            var certificateContextResult = await AttachCertificateAsync(xml, request);
            if (!certificateContextResult.IsSuccess)
                return certificateContextResult;

            var certificateContext = ((ResponseSuccessDto<CertificateResponse>)certificateContextResult).Data;

            // FASE 3 Y 4 - CADENA ORIGINAL Y SELLADO
            var sealResult = SealXml(xml, certificateContext);
            if (!sealResult.IsSuccess)
                return sealResult;

            // FASE 5 – VALIDACIÓN XSD
            var schemaResult = ValidateSchema(xml);
            if (!schemaResult.IsSuccess)
                return schemaResult;

            // FASE 6 – SALDO Y TIMBRADO PAC
            return await StampAsync(xml, certificateContext.UserId, request, ct);
        }
        catch (Exception ex)
        {
            return ResponseFactory.Exception(ex, "Error inesperado al procesar CFDI.");
        }
        

    }


    #region FASE 1 – VALIDACIONES INTERNAS
    /// <summary>
    /// FASE 1 – VALIDACIONES INTERNAS
    /// 1.1 Validación DTO / JSON
    /// 1.2 Reglas fiscales y catálogos SAT
    /// 1.3 Totales
    /// 1.4 Construir XML (sin sello)
    /// 1.5 Validar estructura XML (antes de sellar)
    /// </summary>
    private async Task<ResponseDto> ValidateInternalAsync(CreateCfdiRequest request, CancellationToken ct)
    {
        // 1.1 Validación DTO / JSON
        var validatorCreateCfdiResult = await _validatorCreateCfdiRequest.ValidateAsync(request, ct);
        if (!validatorCreateCfdiResult.IsValid)
        {
            IEnumerable<ResponseErrorDetailDto> errors = validatorCreateCfdiResult.Errors.Select(e => new ResponseErrorDetailDto
            {
                Field = e.PropertyName,
                Message = e.ErrorMessage
            });
            return ResponseFactory.Error(_messagesProvider.GetError("InvalidCfdiStructure"), errors);
        }

        // 1.2 Reglas fiscales y catálogos SAT
        var fiscalRulesValidatorResult = await _cfdiFiscalRulesValidatorService.ExecuteAsync(request, ct);
        if (!fiscalRulesValidatorResult.IsSuccess)
            return fiscalRulesValidatorResult;

        // 1.3 Totales
        var totalsValidatorResult = _cfdiTotalsValidatorService.Execute(request);
        if (!totalsValidatorResult.IsSuccess)
            return totalsValidatorResult;

        // 1.4 Construir XML (sin sello)
        var xml = _xmlBuilder.Build(request);

        // 1.5 Validar estructura XML (antes de sellar)
        var cfdiValidateXmlStructureResult = _cfdiValidateXmlStructure.Execute(xml);
        if (!cfdiValidateXmlStructureResult.IsSuccess)
            return cfdiValidateXmlStructureResult;

        return ResponseFactory.Success<XDocument>(xml, _messagesProvider.GetMessage("InternalValidationPassed"));
    }
    #endregion

    #region FASE 2 - OBTENER CSD, INSERTAR CERTIFICADO Y NOCERTIFICADO
    /// <summary>
    /// FASE 2 - OBTENER CSD, INSERTAR CERTIFICADO Y NOCERTIFICADO
    /// 2.1 Obtener el CSD
    /// 2.2 Obtener el Certificado
    /// 2.3 Insertar Certificado y NoCertificado
    /// </summary>
    private async Task<ResponseDto> AttachCertificateAsync(XDocument xml, CreateCfdiRequest request)
    {
        // 2.1 Obtener el CSD
        var certificateResponse = await _certificateService.GetActiveByRfcAsync(request.Emisor.Rfc);
        if (certificateResponse is null)
        {
            var errors = new List<ResponseErrorDetailDto>();
            errors.Add(new ResponseErrorDetailDto
            {
                Field = "CSD",
                Message = $"No se encontró un CSD activo para el RFC {request.Emisor.Rfc}."
            });            
            return ResponseFactory.Error(_messagesProvider.GetError("CsdNotFound"), errors);
        }        

        // 2.2 Obtener el Certificado
        var certificateResult = _cfdiSealService.Certificate(certificateResponse);
        if (!certificateResult.IsSuccess)
            return certificateResult;

        // 2.3 Insertar Certificado y NoCertificado
        var certificate = ((ResponseSuccessDto<CfdiSealResponse>)certificateResult).Data;

        xml.Root!.SetAttributeValue("Certificado", certificate.Certificado);
        xml.Root.SetAttributeValue("NoCertificado", certificate.NoCertificado);

        return ResponseFactory.Success<CertificateResponse>(certificateResponse, _messagesProvider.GetMessage("CertificateCreatedSuccessfully"));      
    }



    #endregion

    #region FASE 3 Y 4 - CADENA ORIGINAL Y SELLADO
    /// <summary>
    /// FASE 3 Y 4 - CADENA ORIGINAL Y SELLADO
    /// 3.1 Generar cadena original (YA con certificado)
    /// 4.1 Sellar
    /// 4.2 Insertar el sello digital en el XML
    /// </summary>
    private ResponseDto SealXml(XDocument xml, CertificateResponse certificateContext)
    {
        // 3.1 Generar cadena original (YA con certificado)
        var originalStringGeneratorResult = _originalStringGeneratorService.Generate(xml);
        if (!originalStringGeneratorResult.IsSuccess)
            return originalStringGeneratorResult;

        var cadenaOriginal = ((ResponseSuccessDto<string>)originalStringGeneratorResult).Data;

        // 4.1 Sellar
        var sealResult = _cfdiSealService.Seal(cadenaOriginal, certificateContext);
        if (!sealResult.IsSuccess)
            return sealResult;

        // 4.2 Insertar el sello digital en el XML
        var seal = ((ResponseSuccessDto<CfdiSealResponse>)sealResult).Data;
        xml.Root!.SetAttributeValue("Sello", seal.Sello);

        return ResponseFactory.Success(_messagesProvider.GetMessage("SealedSuccessfully"));
    }
    #endregion

    #region FASE 5 - VALIDACIÓN XSD
    /// <summary>
    /// FASE 5 - VALIDACIÓN XSD
    /// </summary>
    private ResponseDto ValidateSchema(XDocument xml)
    {
        var xsdErrors = _xsdValidator.Validate(xml);

        return !xsdErrors.IsSuccess
               ? xsdErrors :
                 ResponseFactory.Success(_messagesProvider.GetMessage("SchemaValidationSuccessfully"));
    }
    #endregion

    #region FASE 6 - VERIFICAR SALDO DE TIMBRES Y TIMBRADO PAC
    /// <summary>
    /// FASE 6 - VERIFICAR SALDO DE TIMBRES Y TIMBRADO PAC
    /// 6.1 Verificar saldo de timbres
    /// 6.2 Timbrar
    /// 6.3 Persistir CFDI timbrado
    /// </summary>
    private async Task<ResponseDto> StampAsync(XDocument xml, Guid userId, CreateCfdiRequest createCfdiRequest, CancellationToken cancellationToken)
    {
        var errors = new List<ResponseErrorDetailDto>();

        //5.1 Verificar saldo de timbres
        var balance = await _stampBalanceRepository.GetByUserIdAsync(userId, false);
        if (balance is null)
        {
            errors.Add(new ResponseErrorDetailDto
            {
                Field = "Saldo de timbres",
                Message = $"No cuenta con saldo suficiente de timbres para realizar el timbrado."
            });
            return ResponseFactory.Error(_messagesProvider.GetError("InsufficientStampBalance"), errors);
        }

        if (balance.AvailableStamps <= 0)
        {
            errors.Add(new ResponseErrorDetailDto
            {
                Field = "Saldo de timbres",
                Message = $"No cuenta con saldo suficiente de timbres para realizar el timbrado."
            });
            return ResponseFactory.Error(_messagesProvider.GetError("InsufficientStampBalance"), errors);
        }

        // 6.2 Timbrar
        string xmlString;
        using (var sw = new StringWriterWithEncoding(new UTF8Encoding(false)))
        {
            xml.Save(sw);
            xmlString = sw.ToString();
        }

        var pacResult = await _pacStampingService.StampingXmlAsync(xmlString, cancellationToken);
        if (!pacResult.IsSuccess)
        {
            errors.Add(new ResponseErrorDetailDto
            {
                Field = "Timbrar CFDI",
                Message = pacResult.Menssage
            });
            return ResponseFactory.Error(_messagesProvider.GetError("CfdiStampedFailed"), errors);
        }

        // 6.3 Persistir CFDI timbrado
        var cfdiPersistenceResult =  await _cfdiPersistenceService.ExecuteAsync(pacResult, createCfdiRequest, userId, balance.Id);
        if(!cfdiPersistenceResult.IsSuccess)
            return cfdiPersistenceResult;


        var createCfdiResponse = new CreateCfdiResponse
        {
            Xml = pacResult.Xml,
            FechaTimbrado = DateTime.UtcNow,
            Uuid = pacResult.Uuid
        };

        return ResponseFactory.Success<CreateCfdiResponse>(createCfdiResponse, "XML timbrado correctamente.");
        
    }

    #endregion




    public class StringWriterWithEncoding : StringWriter
    {
        private readonly Encoding _encoding;

        public StringWriterWithEncoding(Encoding encoding)
        {
            _encoding = encoding;
        }

        public override Encoding Encoding => _encoding;
    }
}
