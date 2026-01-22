namespace FiscalCore.Application.Interfaces.SatCatalog;

public interface ISatCatalogStore
{
    Task<bool> CatalogItemExistsAsync(string catalogCode, string key, CancellationToken ct);
    Task<bool> IsRuleAllowedAsync(string catalogCode, string key, string appliesToCatalog, string appliesToKey, CancellationToken ct);
}
