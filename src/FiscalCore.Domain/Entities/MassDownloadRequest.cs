namespace FiscalCore.Domain.Entities;

public partial class MassDownloadRequest
{
    public Guid Id { get; set; }

    public Guid UserId { get; set; }

    public DateOnly PeriodStart { get; set; }

    public DateOnly PeriodEnd { get; set; }

    public string Status { get; set; } = null!;

    public DateTime CreatedAt { get; set; }
}
