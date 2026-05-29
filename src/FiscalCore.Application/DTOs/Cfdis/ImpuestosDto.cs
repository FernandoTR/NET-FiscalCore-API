namespace FiscalCore.Application.DTOs.Cfdis;

public class ImpuestosDto
{
    public decimal TotalImpuestosTrasladados { get; set; }
    public decimal TotalImpuestosRetenidos { get; set; }

    public List<RetencionGlobalDto> Retenciones { get; set; }
    public List<TrasladoGlobalDto> Traslados { get; set; }
}
