
using FiscalCore.Application.Interfaces.SatCatalog;

namespace FiscalCore.Infrastructure.Persistence.Stores;

public class SatCatalogStore : ISatCatalogStore
{
    private readonly FiscalCoreDbContext _context;

    public SatCatalogStore(FiscalCoreDbContext context)
    {
        _context = context;
    }

    /// <summary>
    /// Valida si un ítem de catálogo SAT existe y está activo.
    /// </summary>
    public async Task<bool> CatalogItemExistsAsync(string catalogCode, string key, CancellationToken ct)
    {
        return await (
            from item in _context.SatCatalogItems.AsNoTracking()
            join catalog in _context.SatCatalogs.AsNoTracking()
                on item.SatCatalogId equals catalog.Id
            where
                catalog.Code == catalogCode &&
                item.KeyCode == key &&
                item.IsActive
            select item.Id
        ).AnyAsync(ct);
    }

    /// <summary>
    /// Valida si una regla SAT permite una combinación específica.
    /// </summary>
    public async Task<bool> IsRuleAllowedAsync(string catalogCode, string key, string appliesToCatalog, string appliesToKey, CancellationToken ct)
    {
        return await _context.SatCatalogRules
        .AnyAsync(r =>
            r.CatalogCode == catalogCode &&
            r.ItemKey == key &&
            r.AppliesToCatalog == appliesToCatalog &&
            r.AppliesToKey == appliesToKey &&
            r.IsAllowed,
            ct);

    }




}
