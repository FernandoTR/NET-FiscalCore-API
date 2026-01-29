
using FiscalCore.Application.Abstractions;
using FiscalCore.Application.DTOs.Cfdis;
using FiscalCore.Application.Interfaces.Cfdis;
using FiscalCore.Application.Interfaces.FileStorage;

namespace FiscalCore.Application.Services.Cfdi;

public sealed class CfdiPdfGenerationService : ICfdiPdfGenerationService
{
    private readonly ICfdiPdfGenerator _pdfGenerator;
    private readonly IFileStorageService _fileStorage;
    private readonly ICfdiPdfStore _cfdiPdfStore;
    private readonly IUnitOfWork _uow;

    public CfdiPdfGenerationService(
        ICfdiPdfGenerator pdfGenerator,
        IFileStorageService fileStorage,
        ICfdiPdfStore cfdiPdfStore,
        IUnitOfWork uow)
    {
        _pdfGenerator = pdfGenerator;
        _fileStorage = fileStorage;
        _cfdiPdfStore = cfdiPdfStore;
        _uow = uow;
    }

    public async Task<CfdiPdfGenerationResult> GenerateAsync(Guid cfdiId, string xmlContent, CancellationToken ct)
    {
        var cfdiPdfId = Guid.NewGuid();
        var fileName = $"{cfdiPdfId}.pdf";

        var pdfBytes = await _pdfGenerator.GenerateAsync(xmlContent);

        var filePath = await _fileStorage.SaveAsync(
            pdfBytes,
            fileName,
            "cfdis/pdfs",
            ct);

        // Calcular versión
        var lastVersion = await _cfdiPdfStore.GetLastVersionAsync(cfdiId);
        var newVersion = lastVersion + 1;

        var pdf = new CfdiPdfDto
        {
            Id = cfdiPdfId,
            CfdiId = cfdiId,
            FilePath = filePath,
            Version = newVersion,
            CreatedAt = DateTime.UtcNow
        };

        _cfdiPdfStore.Add(pdf);
        await _uow.SaveChangesAsync();

        return new CfdiPdfGenerationResult(
            cfdiId,
            cfdiPdfId,
            filePath,
            fileName,
            pdfBytes,
            pdf.Version
        );
    }

}
