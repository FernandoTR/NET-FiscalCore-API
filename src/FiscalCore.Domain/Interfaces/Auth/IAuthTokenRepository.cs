using FiscalCore.Domain.Entities;

namespace FiscalCore.Domain.Interfaces.Auth;

public interface IAuthTokenRepository
{
    Task<AuthToken?> GetByRefreshTokenAsync(string refreshToken);
    void Add(AuthToken token);
    void Revoke(AuthToken token);
}
