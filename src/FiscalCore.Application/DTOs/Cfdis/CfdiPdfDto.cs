

namespace FiscalCore.Application.DTOs.Cfdis;

public class CfdiPdfDto
{
    public Guid Id { get; set; }

    public Guid CfdiId { get; set; }

    public string FilePath { get; set; } = null!;

    public int Version { get; set; }

    public DateTime CreatedAt { get; set; }
}