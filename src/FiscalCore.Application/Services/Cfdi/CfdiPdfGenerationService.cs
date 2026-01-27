
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
        var pdfBytes = await _pdfGenerator.GenerateAsync(xmlContent);

        var filePath = await _fileStorage.SaveAsync(
            pdfBytes,
            $"{cfdiId}.pdf",
            "cfdis/pdfs",
            ct);

        var pdf = new CfdiPdfDto
        {
            Id = Guid.NewGuid(),
            CfdiId = cfdiId,
            FilePath = filePath,
            Version = 1,
            CreatedAt = DateTime.UtcNow
        };

        _cfdiPdfStore.Add(pdf);
        await _uow.SaveChangesAsync();

        return new CfdiPdfGenerationResult(
            cfdiId,
            filePath,
            pdf.Version
        );
    }

}
