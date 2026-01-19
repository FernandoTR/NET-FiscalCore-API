namespace FiscalCore.Domain.Entities;

public partial class StampBalance
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }

    public int TotalStamps { get; set; }
    public int UsedStamps { get; set; }
    public int AvailableStamps => TotalStamps - UsedStamps;

    public DateTime CreatedAt { get; set; }

   
}


