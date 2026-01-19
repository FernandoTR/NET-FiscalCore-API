namespace FiscalCore.Infrastructure.Persistence.Entities;

public partial class CfdiStatusHistory
{
    public Guid Id { get; set; }

    public Guid CfdiId { get; set; }

    public string Status { get; set; } = null!;

    public DateTime ChangedAt { get; set; }

}
