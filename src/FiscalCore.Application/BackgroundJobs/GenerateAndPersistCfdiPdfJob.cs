
using FiscalCore.Application.Interfaces.Cfdis;
using FiscalCore.Application.Interfaces.Logging;

namespace FiscalCore.Application.BackgroundJobs;

public sealed class GenerateAndPersistCfdiPdfJob
{
    private readonly ICfdiPdfGenerationService _pdfGenerationService;
    private readonly ILogService _logService;
    public GenerateAndPersistCfdiPdfJob(
        ICfdiPdfGenerationService pdfGenerationService,
        ILogService logService)
    {
        _pdfGenerationService = pdfGenerationService;
        _logService = logService;
    }

    public async Task ExecuteAsync(Guid cfdiId, string xmlContent, CancellationToken ct)
    {
        try
        {
            await _pdfGenerationService.GenerateAsync(cfdiId, xmlContent, ct);
        }
        catch (Exception ex)
        {
            _logService.ErrorLog($"{nameof(GenerateAndPersistCfdiPdfJob)}.{nameof(ExecuteAsync)}", ex);
            throw;
        }     

     
    }

}
