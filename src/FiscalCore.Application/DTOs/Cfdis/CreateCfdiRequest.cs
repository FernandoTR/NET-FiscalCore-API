namespace FiscalCore.Application.DTOs.Cfdis;

public class CreateCfdiRequest
{
    public ComprobanteDto Comprobante { get; init; }
    public EmisorDto Emisor { get; init; }
    public ReceptorDto Receptor { get; init; }
    public List<ConceptoDto> Conceptos { get; init; }
    public ImpuestosDto? Impuestos { get; init; }
}

