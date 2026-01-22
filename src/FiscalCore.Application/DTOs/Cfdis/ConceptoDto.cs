namespace FiscalCore.Application.DTOs.Cfdis;

public class ConceptoDto
{
    public string ClaveProdServ { get; set; }
    public string ClaveUnidad { get; set; }
    public string Unidad { get; set; }
    public string Descripcion { get; set; }
    public decimal Cantidad { get; set; }
    public decimal ValorUnitario { get; set; }
    public decimal Importe { get; set; }
    public decimal? Descuento { get; set; }
    public string ObjetoImp { get; set; }
    public ConceptoImpuestosDto? Impuestos { get; init; }
}
