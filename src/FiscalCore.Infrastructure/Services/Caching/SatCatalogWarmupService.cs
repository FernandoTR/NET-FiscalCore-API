
using FiscalCore.Application.Interfaces.Caching;
using Microsoft.Extensions.Caching.Memory;

namespace FiscalCore.Infrastructure.Services.Caching;

public sealed class SatCatalogWarmupService : ISatCatalogWarmupService
{
    private readonly IMemoryCache _memoryCache;
    private readonly FiscalCoreDbContext _dbContext;

    private static readonly TimeSpan CacheDuration = TimeSpan.FromHours(12);

    public SatCatalogWarmupService(IMemoryCache memoryCache, FiscalCoreDbContext dbContext)
    {
        _memoryCache = memoryCache;
        _dbContext = dbContext;
    }

    public async Task WarmupAsync(CancellationToken ct)
    {
        // 1️ Cargar items de catálogo
        var items =  await (
           from item in _dbContext.SatCatalogItems.AsNoTracking()
           join catalog in _dbContext.SatCatalogs.AsNoTracking()
               on item.SatCatalogId equals catalog.Id
           where
               item.IsActive
           select new
           {
               CatalogCode = catalog.Code,
               item.KeyCode
           }).ToListAsync(ct);

        foreach (var item in items)
        {
            var cacheKey = $"SAT:{item.CatalogCode}:{item.KeyCode}";
            _memoryCache.Set(cacheKey, true, CacheDuration);
        }

        // 2️ Cargar reglas cruzadas
        var rules = await _dbContext.SatCatalogRules
            .Where(r => r.IsAllowed)
            .Select(r => new
            {
                r.CatalogCode,
                r.ItemKey,
                r.AppliesToCatalog,
                r.AppliesToKey
            })
            .ToListAsync(ct);

        foreach (var rule in rules)
        {
            var cacheKey =
                $"SAT_RULE:{rule.CatalogCode}:{rule.ItemKey}:{rule.AppliesToCatalog}:{rule.AppliesToKey}";

            _memoryCache.Set(cacheKey, true, CacheDuration);
        }
    }



}
