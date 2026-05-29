using FiscalCore.Domain.Interfaces.Auth;

namespace FiscalCore.Infrastructure.Persistence.Repositories;

public sealed class AuthTokenRepository : IAuthTokenRepository
{
    private readonly FiscalCoreDbContext _context;

    public AuthTokenRepository(FiscalCoreDbContext context)
    {
        _context = context;
    }

    public async Task<AuthToken?> GetByRefreshTokenAsync(string refreshToken)
    {
        return await _context.AuthTokens.FirstOrDefaultAsync(t => t.Token == refreshToken && !t.IsRevoked);
    }

    public void Add(AuthToken token)
    {
        _context.AuthTokens.Add(token);
    }

    public void Revoke(AuthToken token)
    {
        token.IsRevoked = true;
        _context.AuthTokens.Update(token);
    }
}
