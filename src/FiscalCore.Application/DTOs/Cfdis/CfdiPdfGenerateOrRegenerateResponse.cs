namespace FiscalCore.Application.DTOs.Cfdis;
public sealed record CfdiPdfGenerateOrRegenerateResponse(
    Guid Uuid,
    byte[] PdfBytes,
    int Version
);