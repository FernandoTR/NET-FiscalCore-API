using FiscalCore.Application.Interfaces.SatCatalog;
using FiscalCore.Infrastructure.Persistence.Stores;
using FiscalCore.Infrastructure.Services.Caching;
using FiscalCore.Infrastructure.Services.Hosting;
using Microsoft.Extensions.Caching.Memory;

namespace FiscalCore.Api;

public static class DependencyInjection
{
    public static IServiceCollection AddWeb(this IServiceCollection services)
    {
        // Decorator MemoryCache for Sat Catalog
        services.AddScoped<ISatCatalogStore>(sp =>
        {
            var baseService = sp.GetRequiredService<SatCatalogStore>();
            var cache = sp.GetRequiredService<IMemoryCache>();

            return new CachedSatCatalogService(baseService, cache);
        });

        // Warmup for Sat Catalog
        services.AddHostedService<SatCatalogWarmupHostedService>();

        return services;
    }
}
