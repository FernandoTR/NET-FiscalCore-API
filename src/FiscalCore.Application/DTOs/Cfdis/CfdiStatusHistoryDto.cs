namespace FiscalCore.Application.DTOs.Cfdis;

public class CfdiStatusHistoryDto
{
    public Guid Id { get; set; }
    public Guid CfdiId { get; set; }
    public string Status { get; set; } = null!;
    public DateTime ChangedAt { get; set; }
}
