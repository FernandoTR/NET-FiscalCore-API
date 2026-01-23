namespace FiscalCore.Application.DTOs.Pac;

public partial class PacAuthResponse
{
    public Resultado Resultado { get; set; }
}

public partial class Resultado
{
    public string Estatus { get; set; }
    public long LatenciaMs { get; set; }
    public DateTimeOffset FechaTransaccion { get; set; }
    public long IdTransaccion { get; set; }
    public string Usuario { get; set; }
    public Token Token { get; set; }
}

public partial class Token
{
    public string Tipo { get; set; }
    public string Datos { get; set; }
    public long Longitud { get; set; }
    public string Expedido { get; set; }
    public string Expira { get; set; }
}
