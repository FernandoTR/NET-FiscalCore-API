using FiscalCore.Application.DTOs.Auth;

namespace FiscalCore.Application.Interfaces.Auth;

public interface IJwtTokenService
{
    JwtTokenDto GenerateToken(User user);
}