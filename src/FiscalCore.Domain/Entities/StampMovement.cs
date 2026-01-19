namespace FiscalCore.Domain.Entities;

public partial class StampMovement
{
    public Guid Id { get; set; }
    public Guid StampBalanceId { get; set; }
    public Guid CfdiId { get; set; }
    public int Amount { get; set; }
    public DateTime CreatedAt { get; set; }

}
