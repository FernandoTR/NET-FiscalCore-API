using FiscalCore.Application.DTOs.Cfdis;

namespace FiscalCore.Application.Interfaces.Cfdis;

public interface ICfdiPdfGenerationService
{
    Task<CfdiPdfGenerationResult> GenerateAsync(Guid cfdiId, string xmlContent, CancellationToken ct);
}
