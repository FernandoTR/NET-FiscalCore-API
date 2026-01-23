namespace FiscalCore.Application.DTOs.Cfdis;

public class ComprobanteDto
{
    public string Version { get; set; } // 4.0
    public string FormaPago { get; set; } // 01, 03, 99  
    public string Serie { get; set; }
    public string Folio { get; set; }
    public DateTime Fecha { get; set; }
    public string MetodoPago { get; init; }  // PUE, PPD   
    public string Sello { get; set; }
    public string NoCertificado { get; set; }
    public string Certificado { get; set; }
    public string CondicionesDePago { get; set; }
    public decimal SubTotal { get; set; }
    public decimal Descuento { get; set; }
    public string Moneda { get; set; }  // MXN, USD, etc.
    public string TipoCambio { get; set; }
    public decimal Total { get; set; }
    public string TipoDeComprobante { get; set; } // I, E, T, N, P
    public string Exportacion { get; set; }
    public string LugarExpedicion { get; set; } // CP

}
