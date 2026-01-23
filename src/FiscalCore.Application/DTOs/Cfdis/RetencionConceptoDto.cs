namespace FiscalCore.Application.DTOs.Cfdis;

public class RetencionConceptoDto
{
    public decimal Base { get; set; }
    public decimal Importe { get; set; }
    public string Impuesto { get; set; }
    public decimal TasaOCuota { get; set; }
    public string TipoFactor { get; set; }
}
