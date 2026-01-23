namespace FiscalCore.Application.DTOs.Cfdis;

public class CfdiXmlDto
{
    public Guid Id { get; set; }

    public Guid CfdiId { get; set; }

    public string XmlContent { get; set; } = null!;

    public DateTime CreatedAt { get; set; }
}
