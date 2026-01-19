namespace FiscalCore.Infrastructure.Persistence.Entities;

public partial class ActivityLog
{
    public long Id { get; set; }

    public DateTime LogDate { get; set; }

    public string EventType { get; set; } = null!;

    public string Description { get; set; } = null!;

    public Guid UserId { get; set; }

}
