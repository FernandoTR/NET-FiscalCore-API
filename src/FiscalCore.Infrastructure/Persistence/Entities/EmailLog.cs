namespace FiscalCore.Infrastructure.Persistence.Entities;

public partial class EmailLog
{
    public Guid Id { get; set; }

    public Guid CfdiId { get; set; }

    public string Recipient { get; set; } = null!;

    public DateTime SentAt { get; set; }

}
