namespace FiscalCore.Application.DTOs.Cfdis;

public sealed record CfdiPdfGenerationResult(
    Guid CfdiId,
    string FilePath,
    int Version
);

