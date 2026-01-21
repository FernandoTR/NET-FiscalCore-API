using FiscalCore.Application.Abstractions;
using FiscalCore.Application.Interfaces.Auth;
using FiscalCore.Application.Interfaces.Logging;
using FiscalCore.Application.Interfaces.Message;
using FiscalCore.Application.Interfaces.Security;
using FiscalCore.Domain.Interfaces.Auth;
using FiscalCore.Domain.Interfaces.Certificates;
using FiscalCore.Domain.Interfaces.Users;
using FiscalCore.Infrastructure.Logging;
using FiscalCore.Infrastructure.Persistence;
using FiscalCore.Infrastructure.Persistence.Repositories;
using FiscalCore.Infrastructure.Security.Encryption;
using FiscalCore.Infrastructure.Security.Jwt;
using FiscalCore.Infrastructure.Services.MessagesProvider;
using Microsoft.Extensions.DependencyInjection;

namespace FiscalCore.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, string connectionString)
    {
        services.AddScoped<IEncryptionService, AesEncryptionService>();
        services.AddScoped<ILogService, LogService>();
        services.AddScoped<IMessagesProvider, ResxMessagesProvider>();

        // DbContext
        services.AddDbContext<FiscalCoreDbContext>(options => options.UseSqlServer(connectionString));
        services.AddDbContext<LoggingDbContext>(options => options.UseSqlServer(connectionString));
        services.AddScoped<IUnitOfWork, EfUnitOfWork>();

        // Security
        services.AddScoped<IJwtTokenService, JwtTokenService>();

        // Repository
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IAuthTokenRepository, AuthTokenRepository>();
        services.AddScoped<ICertificateRepository, CertificateRepository>();





        return services;
    }
}
