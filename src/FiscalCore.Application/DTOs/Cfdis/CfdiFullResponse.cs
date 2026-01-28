namespace FiscalCore.Application.DTOs.Cfdis;

public class CfdiFullResponse
{
    public Guid UserId { get; set; }
    public Guid Uuid { get; set; }
    public string Folio { get; set; } = null!;
    public string Version { get; set; } = null!;
    public string Serie { get; set; } = null!;
    public string Status { get; set; } = null!;
    public DateTime Fecha { get; set; }
    public DateTime FechaTimbrado { get; set; }
    public string LugarExpedicion { get; set; } = null!;
    public string TipoComprobante { get; set; } = null!;
    public string NumeroCertificado { get; set; } = null!;
    public string NumeroSerie { get; set; } = null!;
    public string FormaPago { get; set; } = null!;
    public string MetodoPago { get; set; } = null!;
    public string UsoCFDI { get; set; } = null!;
    public string moneda { get; set; } = null!;
    public string tipoCambio { get; set; } = null!;    
    public decimal SubTotal { get; set; }
    public decimal Descuento { get; set; }
    public decimal Total { get; set; }
    public decimal TotalImpuestosTraslados { get; set; }
    public decimal TotalImpuestosRetencion { get; set; }
    public string EmisorRFC { get; set; } = null!;
    public string EmisorNombre { get; set; } = null!;
    public string EmisorRegimenFiscal { get; set; } = null!;
    public string ReceptorRFC { get; set; } = null!;
    public string ReceptorNombre { get; set; } = null!;
    public string ReceptoDomicilioFiscal { get; set; } = null!;
    public string SelloCFD { get; set; } = null!;
    public string Xml { get; set; } = null!;
}
