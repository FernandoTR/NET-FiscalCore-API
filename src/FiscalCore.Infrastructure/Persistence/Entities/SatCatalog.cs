namespace FiscalCore.Infrastructure.Persistence.Entities;

public class SatCatalog
{
    public Guid Id { get; set; }

    /// <summary>
    /// Código SAT del catálogo (ej. c_FormaPago)
    /// </summary>
    public string Code { get; set; } = default!;

    /// <summary>
    /// Nombre legible del catálogo
    /// </summary>
    public string Name { get; set; } = default!;

    public string? Description { get; set; }

    /// <summary>
    /// Versión CFDI (4.0)
    /// </summary>
    public string CfdiVersion { get; set; } = default!;

    public bool IsActive { get; set; }

    public DateTime CreatedAt { get; set; }

}
