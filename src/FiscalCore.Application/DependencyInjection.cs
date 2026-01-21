using FiscalCore.Application.Interfaces.Auth;
using FiscalCore.Application.Interfaces.Users;
using FiscalCore.Application.Services.Auth;
using FiscalCore.Application.Services.User;
using Microsoft.Extensions.DependencyInjection;

namespace FiscalCore.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<IUserService, UserService>();



        return services;
    }
}
