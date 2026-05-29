namespace FiscalCore.Infrastructure.Persistence.Entities;

public class SatCatalogRule
{
    public Guid Id { get; set; }

    /// <summary>
    /// Catálogo origen (ej. c_FormaPago)
    /// </summary>
    public string CatalogCode { get; set; } = default!;

    /// <summary>
    /// Clave origen (ej. 99)
    /// </summary>
    public string ItemKey { get; set; } = default!;

    /// <summary>
    /// Catálogo destino (ej. c_MetodoPago)
    /// </summary>
    public string AppliesToCatalog { get; set; } = default!;

    /// <summary>
    /// Clave destino (ej. PPD)
    /// </summary>
    public string AppliesToKey { get; set; } = default!;

    /// <summary>
    /// Indica si la combinación es válida
    /// </summary>
    public bool IsAllowed { get; set; }

    public DateTime CreatedAt { get; set; }
}
