
using FiscalCore.Application.DTOs.Auth;

namespace FiscalCore.Application.Interfaces.Auth;

public interface IAuthService
{
    Task<LoginResponse?> LoginAsync(LoginRequest request);
    Task<RefreshTokenResponse?> RefreshTokenAsync(string refreshToken);
}
