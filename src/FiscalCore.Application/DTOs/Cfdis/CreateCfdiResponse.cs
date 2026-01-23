namespace FiscalCore.Application.DTOs.Cfdis;

public class CreateCfdiResponse
{
    public string Uuid { get; set; }
    public string Xml { get; set; }
    public DateTime FechaTimbrado { get; set; }
}
