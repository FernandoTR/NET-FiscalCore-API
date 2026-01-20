using FiscalCore.Application.Interfaces.Auth;
using FiscalCore.Application.Services.Auth;
using Microsoft.Extensions.DependencyInjection;

namespace FiscalCore.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddScoped<IAuthService, AuthService>();



        return services;
    }
}
