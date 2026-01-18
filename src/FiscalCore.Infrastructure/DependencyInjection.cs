using FiscalCore.Application.Interfaces.Security;
using FiscalCore.Infrastructure.Security.Encryption;
using Microsoft.Extensions.DependencyInjection;

namespace FiscalCore.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services)
    {
        services.AddScoped<IEncryptionService, AesEncryptionService>();

        return services;
    }
}
