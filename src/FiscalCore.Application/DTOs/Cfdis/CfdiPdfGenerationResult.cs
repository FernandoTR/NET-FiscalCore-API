namespace FiscalCore.Application.DTOs.Cfdis;

public sealed record CfdiPdfGenerationResult(
    Guid CfdiId,
    Guid CfdiPdfId,
    string FilePath,
    string FileName,
    byte[] PdfBytes,
    int Version
);

