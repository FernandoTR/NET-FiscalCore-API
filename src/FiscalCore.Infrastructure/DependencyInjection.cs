using FiscalCore.Application.Interfaces.Security;
using FiscalCore.Infrastructure.Persistence.Context;
using FiscalCore.Infrastructure.Security.Encryption;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace FiscalCore.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, string connectionString)
    {
        services.AddScoped<IEncryptionService, AesEncryptionService>();

        // DbContext
        services.AddDbContext<FiscalCoreDbContext>(options => options.UseSqlServer(connectionString));
        services.AddDbContext<LoggingDbContext>(options => options.UseSqlServer(connectionString));



        return services;
    }
}
