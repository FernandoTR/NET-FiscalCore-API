using FiscalCore.Application.Interfaces.Caching;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace FiscalCore.Infrastructure.Services.Hosting;

public sealed class SatCatalogWarmupHostedService : IHostedService
{
    private readonly IServiceProvider _serviceProvider;

    public SatCatalogWarmupHostedService(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        using var scope = _serviceProvider.CreateScope();

        var warmupService = scope.ServiceProvider.GetRequiredService<ISatCatalogWarmupService>();

        await warmupService.WarmupAsync(cancellationToken);
    }

    public Task StopAsync(CancellationToken cancellationToken)
        => Task.CompletedTask;
}
