namespace FiscalCore.Infrastructure.Persistence.Entities;

public partial class CfdiXml
{
    public Guid Id { get; set; }

    public Guid CfdiId { get; set; }

    public string XmlContent { get; set; } = null!;

    public DateTime CreatedAt { get; set; }

}
