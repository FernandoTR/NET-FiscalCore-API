using FiscalCore.Application.BackgroundJobs;
using FiscalCore.Application.Interfaces.Auth;
using FiscalCore.Application.Interfaces.Certificate;
using FiscalCore.Application.Interfaces.Cfdis;
using FiscalCore.Application.Interfaces.Users;
using FiscalCore.Application.Services.Auth;
using FiscalCore.Application.Services.Certificate;
using FiscalCore.Application.Services.Cfdi;
using FiscalCore.Application.Services.User;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace FiscalCore.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        // Register FluentValidation validators from the FiscalCore.Application assembly
        services.AddValidatorsFromAssembly(Assembly.Load("FiscalCore.Application"));


        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<IUserService, UserService>();
        services.AddScoped<ICertificateService, CertificateService>();
        services.AddScoped<ICfdiFiscalRulesValidatorService, CfdiFiscalRulesValidatorService>();
        services.AddScoped<ICfdiTotalsValidatorService, CfdiTotalsValidatorService>();
        services.AddScoped<ICfdiPersistenceService, CfdiPersistenceService>();
        services.AddScoped<ICreateAndStampCfdiService, CreateAndStampCfdiService>();
        services.AddScoped<ICfdiPdfGenerationService, CfdiPdfGenerationService>();
        services.AddScoped<ICfdiQueryService, CfdiQueryService>();
        services.AddScoped<ICfdiPdfGenerateOrRegenerateService, CfdiPdfGenerateOrRegenerateService>();


        // Register Background Jobs
        services.AddScoped<GenerateAndPersistCfdiPdfJob>();
        services.AddScoped<SendCfdiEmailJob>();




        return services;
    }
}
