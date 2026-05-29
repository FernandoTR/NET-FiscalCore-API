namespace FiscalCore.Application.DTOs.Cfdis;

public class CfdiDto
{
    public Guid Id { get; set; }

    public Guid UserId { get; set; }

    public Guid Uuid { get; set; }

    public string RfcEmisor { get; set; } = null!;

    public string RfcReceptor { get; set; } = null!;

    public string Version { get; set; } = null!;

    public decimal Total { get; set; }

    public string Currency { get; set; } = null!;

    public DateTime IssueDate { get; set; }

    public string CurrentStatus { get; set; } = null!;

    public DateTime CreatedAt { get; set; }
}
