namespace FiscalCore.Infrastructure.Persistence.Entities;

public partial class CfdiPdf
{
    public Guid Id { get; set; }

    public Guid CfdiId { get; set; }

    public string FilePath { get; set; } = null!;

    public int Version { get; set; }

    public DateTime CreatedAt { get; set; }

}
