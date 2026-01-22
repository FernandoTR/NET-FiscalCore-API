using FiscalCore.Application.Abstractions;
using FiscalCore.Application.Interfaces.Auth;
using FiscalCore.Application.Interfaces.Cfdis;
using FiscalCore.Application.Interfaces.Logging;
using FiscalCore.Application.Interfaces.Message;
using FiscalCore.Application.Interfaces.Pac;
using FiscalCore.Application.Interfaces.SatCatalog;
using FiscalCore.Application.Interfaces.Security;
using FiscalCore.Domain.Interfaces.Auth;
using FiscalCore.Domain.Interfaces.Certificates;
using FiscalCore.Domain.Interfaces.Cfdis;
using FiscalCore.Domain.Interfaces.Stamping;
using FiscalCore.Domain.Interfaces.Users;
using FiscalCore.Infrastructure.CfdiBuilder;
using FiscalCore.Infrastructure.Logging;
using FiscalCore.Infrastructure.Pac;
using FiscalCore.Infrastructure.Persistence.Repositories;
using FiscalCore.Infrastructure.Persistence.Stores;
using FiscalCore.Infrastructure.Persistence.UnitOfWork;
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
        services.AddScoped<ICfdiRepository, CfdiRepository>();
        services.AddScoped<IStampBalanceRepository, StampBalanceRepository>();
        services.AddScoped<IStampMovementRepository, StampMovementRepository>();


        // Stores
        services.AddScoped<ICfdiXmlStore, CfdiXmlStore>();
        services.AddScoped<ICfdiPdfStore, CfdiPdfStore>();
        services.AddScoped<ICfdiStatusHistoryStore, CfdiStatusHistoryStore>();
        services.AddScoped<ISatCatalogStore, SatCatalogStore>();


        services.AddScoped<IOriginalStringGeneratorService, OriginalStringGeneratorService>();
        services.AddScoped<ICfdiXsdValidator, CfdiXsdValidator>();
        services.AddScoped<ICfdiSealService, CfdiSealService>();
        services.AddScoped<ICfdiValidateXmlStructure, CfdiValidateXmlStructure>();
        services.AddScoped<ICfdiXmlBuilder, CfdiXmlBuilder>();

        // Pac
        services.AddScoped<IPacStampingService, FakeStampingService>();



        return services;
    }
}
