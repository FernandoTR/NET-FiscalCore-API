
using FiscalCore.Application.Interfaces.SatCatalog;
using Microsoft.Extensions.Caching.Memory;

namespace FiscalCore.Infrastructure.Services.Caching;

public sealed class CachedSatCatalogService : ISatCatalogStore
{
    private readonly ISatCatalogStore _inner;
    private readonly IMemoryCache _cache;

    private static readonly TimeSpan CacheDuration = TimeSpan.FromHours(12);

    public CachedSatCatalogService(ISatCatalogStore inner, IMemoryCache cache)
    {
        _inner = inner;
        _cache = cache;
    }

    public async Task<bool> CatalogItemExistsAsync(string catalogCode, string key, CancellationToken ct)
    {
        var cacheKey = $"SAT:{catalogCode}:{key}";

        if (_cache.TryGetValue(cacheKey, out bool exists))
            return exists;

        exists = await _inner.CatalogItemExistsAsync(catalogCode, key, ct);

        _cache.Set(cacheKey, exists, CacheDuration);

        return exists;
    }

    public async Task<bool> IsRuleAllowedAsync(
        string catalogCode,
        string key,
        string appliesToCatalog,
        string appliesToKey,
        CancellationToken ct)
    {
        var cacheKey =
            $"SAT_RULE:{catalogCode}:{key}:{appliesToCatalog}:{appliesToKey}";

        if (_cache.TryGetValue(cacheKey, out bool allowed))
            return allowed;

        allowed = await _inner.IsRuleAllowedAsync(
            catalogCode, key, appliesToCatalog, appliesToKey, ct);

        _cache.Set(cacheKey, allowed, CacheDuration);

        return allowed;
    }
}
