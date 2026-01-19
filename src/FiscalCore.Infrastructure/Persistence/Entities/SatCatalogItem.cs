namespace FiscalCore.Infrastructure.Persistence.Entities;

public class SatCatalogItem
{
    public Guid Id { get; set; }

    public Guid SatCatalogId { get; set; }

    /// <summary>
    /// Clave SAT (ej. 01, PUE, G01)
    /// </summary>
    public string KeyCode { get; set; } = default!;

    public string Description { get; set; } = default!;

    /// <summary>
    /// Inicio de vigencia SAT
    /// </summary>
    public DateTime StartDate { get; set; }

    /// <summary>
    /// Fin de vigencia SAT (null = vigente)
    /// </summary>
    public DateTime? EndDate { get; set; }

    public bool IsActive { get; set; }

    public DateTime CreatedAt { get; set; }

}
