namespace FiscalCore.Application.Interfaces.Caching;

public interface ISatCatalogWarmupService
{
    Task WarmupAsync(CancellationToken ct);
}
