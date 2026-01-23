namespace FiscalCore.Application.DTOs.Cfdis;

public class ConceptoImpuestosDto
{
    public List<TrasladoConceptoDto>? Traslados { get; set; }
    public List<RetencionConceptoDto>? Retenciones { get; set; }
}
