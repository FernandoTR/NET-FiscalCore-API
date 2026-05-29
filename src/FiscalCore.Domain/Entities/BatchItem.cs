namespace FiscalCore.Domain.Entities;

public partial class BatchItem
{
    public Guid Id { get; set; }

    public Guid BatchId { get; set; }

    public Guid CfdiId { get; set; }

    public string Status { get; set; } = null!;
}
