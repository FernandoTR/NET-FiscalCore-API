
using FiscalCore.Application.DTOs.Cfdis;
using FiscalCore.Application.DTOs.Common;
using FiscalCore.Application.Interfaces.Cfdis;
using FiscalCore.Application.Interfaces.Logging;
using FiscalCore.Domain.Interfaces.Cfdis;

namespace FiscalCore.Application.Services.Cfdi;

public sealed class CfdiPdfGenerateOrRegenerateService : ICfdiPdfGenerateOrRegenerateService
{
    private readonly ICfdiPdfGenerationService _pdfGenerationService;
    private readonly ILogService _logService;
    private readonly ICfdiRepository _cfdiRepository;
    private readonly ICfdiXmlStore _cfdiXmlStore;

    public CfdiPdfGenerateOrRegenerateService(
        ICfdiPdfGenerationService pdfGenerationService,
        ILogService logService,
        ICfdiRepository cfdiRepository,
        ICfdiXmlStore cfdiXmlStore)
    {
        _pdfGenerationService = pdfGenerationService;
        _logService = logService;
        _cfdiRepository = cfdiRepository;
        _cfdiXmlStore = cfdiXmlStore;
    }

    public async Task<ResponseDto> ExecuteAsync(Guid uuid, CancellationToken cancellationToken)
    {
        try
        {
            // 1️ Obtener CFDI
            var cfdi = await _cfdiRepository.GetByUuidAsync(uuid);
            if (cfdi is null)
                return ResponseFactory.Error($"CFDI {uuid} no existe.");

            // 2️ Obtener XML (si no fue enviado)
            string cfdiXml;

            var storedXml = await _cfdiXmlStore.GetByCfdiIdAsync(cfdi.Id);

            if (storedXml is null)
                return ResponseFactory.Error("El CFDI no tiene XML asociado.");

            cfdiXml = storedXml.XmlContent;


            // 3️ Generar PDF
            var generationResult = await _pdfGenerationService.GenerateAsync(cfdi.Id, cfdiXml, cancellationToken);
            if (generationResult is null)
                return ResponseFactory.Error("Error de generación de CfdiPdf");

          
            return ResponseFactory.Success(new CfdiPdfGenerateOrRegenerateResponse(uuid,generationResult.PdfBytes,generationResult.Version), "Pdf generado con éxito.");
        }
        catch (Exception ex)
        {
            _logService.ErrorLog($"{nameof(CfdiPdfGenerateOrRegenerateService)}.{nameof(ExecuteAsync)}", ex);
            return ResponseFactory.Exception(ex, "Se produjo un error al intentar generar el pdf.");
        }
    }
}
