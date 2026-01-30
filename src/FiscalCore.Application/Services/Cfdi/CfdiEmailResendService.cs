using FiscalCore.Application.DTOs.Cfdis;
using FiscalCore.Application.DTOs.Common;
using FiscalCore.Application.DTOs.Email;
using FiscalCore.Application.Interfaces.Cfdis;
using FiscalCore.Application.Interfaces.Email;
using FiscalCore.Application.Interfaces.Logging;
using FiscalCore.Application.Interfaces.Message;
using FiscalCore.Domain.Interfaces.Cfdis;
using FluentValidation;

namespace FiscalCore.Application.Services.Cfdi;

public class CfdiEmailResendService : ICfdiEmailResendService
{
    private readonly IEmailService _emailService;
    private readonly ICfdiRepository _cfdiRepository;
    private readonly ICfdiXmlStore _cfdiXmlStore;
    private readonly ICfdiPdfStore _cfdiPdfStore;
    private readonly ILogService _logService;
    private readonly IMessagesProvider _messagesProvider;
    private readonly IValidator<CfdiEmailResendRequest> _validatorCfdiEmailResendRequest;

    public CfdiEmailResendService(
        IEmailService emailService,
        ICfdiRepository cfdiRepository,
        ICfdiXmlStore cfdiXmlStore,
        ICfdiPdfStore cfdiPdfStore,
        ILogService logService,
        IMessagesProvider messagesProvider,
        IValidator<CfdiEmailResendRequest> validator)
    {
        _emailService = emailService;
        _cfdiRepository = cfdiRepository;
        _cfdiXmlStore = cfdiXmlStore;
        _cfdiPdfStore = cfdiPdfStore;
        _logService = logService;
        _messagesProvider = messagesProvider;
        _validatorCfdiEmailResendRequest = validator;
    }

    public async Task<ResponseDto> ResendAsync(CfdiEmailResendRequest request, CancellationToken ct)
    {
        // 1 Validación DTO
        var validatorResult = await _validatorCfdiEmailResendRequest.ValidateAsync(request, ct);
        if (!validatorResult.IsValid)
        {
            IEnumerable<ResponseErrorDetailDto> errors = validatorResult.Errors.Select(e => new ResponseErrorDetailDto
            {
                Field = e.PropertyName,
                Message = e.ErrorMessage
            });
            return ResponseFactory.Error(_messagesProvider.GetError("IncompleteFields"), errors);
        }

       

        try
        {
            var cfdi = await _cfdiRepository.GetByUuidAsync(request.Uuid);
            if (cfdi == null)
            {
                IEnumerable<ResponseErrorDetailDto> errors = validatorResult.Errors.Select(e => new ResponseErrorDetailDto
                {
                    Field = "Uuid",
                    Message = "Cfdi no encontrado"
                });
                return ResponseFactory.Error(_messagesProvider.GetError("CfdiNotFound"), errors);
            }

            var cfdiXml = await _cfdiXmlStore.GetByCfdiIdAsync(cfdi.Id);
            if (cfdi == null)
            {
                IEnumerable<ResponseErrorDetailDto> errors = validatorResult.Errors.Select(e => new ResponseErrorDetailDto
                {
                    Field = "xml",
                    Message = "xml no encontrado"
                });
                return ResponseFactory.Error(_messagesProvider.GetError("CfdiNotFound"), errors);
            }

            var cfdiPdf = await _cfdiPdfStore.GetByCfdiIdAsync(cfdi.Id);
            if (cfdiPdf == null)
            {
                IEnumerable<ResponseErrorDetailDto> errors = validatorResult.Errors.Select(e => new ResponseErrorDetailDto
                {
                    Field = "pdf",
                    Message = "pdf no encontrado"
                });
                return ResponseFactory.Error(_messagesProvider.GetError("CfdiNotFound"), errors);
            }

            await _emailService.SendAsync(
              to: request.To,
              subject: "CFDI generado",
              body: "Se adjuntan los archivos XML y PDF de su CFDI.",
              attachments: new[]
              {
                    EmailAttachment.FromBytes(
                        $"{cfdi.Id}.xml",
                        System.Text.Encoding.UTF8.GetBytes(cfdiXml.XmlContent),
                        "application/xml"
                    ),
                    EmailAttachment.FromFile(
                        cfdiPdf.FilePath,
                        contentType: "application/pdf"
                    )
              },
              ct);


            return ResponseFactory.Success(_messagesProvider.GetMessage("CfdiResendSuccess"));

        }
        catch (Exception ex )
        {
            _logService.ErrorLog($"{nameof(CfdiEmailResendService)}.{nameof(ResendAsync)}", ex);

            return ResponseFactory.Exception(ex, _messagesProvider.GetError("CfdiResendFailed"));
        }

    }
}
