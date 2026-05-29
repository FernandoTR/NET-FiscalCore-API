namespace FiscalCore.Domain.Entities;

public partial class CfdiBatch
{
    public Guid Id { get; set; }

    public Guid UserId { get; set; }

    public string Status { get; set; } = null!;

    public DateTime CreatedAt { get; set; }
}
