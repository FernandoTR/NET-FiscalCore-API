namespace FiscalCore.Application.Interfaces.Cfdis;

public interface ICfdiPdfGenerator
{
    Task<byte[]> GenerateAsync(string xmlContent);
}

