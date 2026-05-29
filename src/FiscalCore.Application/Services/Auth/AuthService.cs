using FiscalCore.Application.Abstractions;
using FiscalCore.Application.DTOs.Auth;
using FiscalCore.Application.Interfaces.Auth;
using FiscalCore.Application.Interfaces.Logging;
using FiscalCore.Application.Interfaces.Security;
using FiscalCore.Domain.Interfaces.Auth;
using FiscalCore.Domain.Interfaces.Users;

namespace FiscalCore.Application.Services.Auth;

public class AuthService : IAuthService
{
    private readonly ILogService _logService;
    private readonly IUserRepository _userRepository;
    private readonly IAuthTokenRepository _authTokenRepository;
    private readonly IJwtTokenService _jwtTokenService;
    private readonly IEncryptionService _encryptionService;
    private readonly IUnitOfWork _uow;

    public AuthService(
        ILogService logService,
        IUserRepository userRepository,
        IAuthTokenRepository authTokenRepository,
        IJwtTokenService jwtTokenService,
        IUnitOfWork unitOfWork,
        IEncryptionService encryptionService)
    {
        _logService = logService;
        _userRepository = userRepository;
        _authTokenRepository = authTokenRepository;
        _jwtTokenService = jwtTokenService;
        _uow = unitOfWork;
        _encryptionService = encryptionService;
    }


    public async Task<LoginResponse?> LoginAsync(LoginRequest request)
    {
        try
        {
            var user = await _userRepository.GetByEmailAsync(request.Email);
            if (user is null)
                return null;

            var passwordHasher = _encryptionService.Decrypt(user.PasswordHash);
            if (passwordHasher != request.Password)
                return null;

            await _uow.BeginTransactionAsync();

            var accessToken = _jwtTokenService.GenerateToken(user);

            var refreshToken = new AuthToken
            {
                Id = Guid.NewGuid(),
                UserId = user.Id,
                Token = Guid.NewGuid().ToString("N"),
                CreatedAt = DateTime.UtcNow,
                ExpiresAt = DateTime.UtcNow.AddDays(7),
                IsRevoked = false
            };

            _authTokenRepository.Add(refreshToken);

            await _uow.SaveChangesAsync();
            await _uow.CommitAsync();

            return new LoginResponse(
                accessToken.AccessToken,
                accessToken.TokenType,
                accessToken.IssuedAt,
                accessToken.ExpiresAt,
                refreshToken.Token
            );
        }
        catch (Exception ex)
        {
            await _uow.RollbackAsync();
            _logService.ErrorLog($"{nameof(AuthService)}.{nameof(LoginAsync)}", ex);
            return null;
        }

    }

    public async Task<RefreshTokenResponse?> RefreshTokenAsync(string refreshToken)
    {
        try
        {
            var storedToken = await _authTokenRepository.GetByRefreshTokenAsync(refreshToken);

            if (storedToken is null ||
                storedToken.IsRevoked ||
                storedToken.ExpiresAt < DateTime.UtcNow)
                return null;

            var user = await _userRepository.GetByIdAsync(storedToken.UserId);

            await _uow.BeginTransactionAsync();

            // revoca el token enviado
            _authTokenRepository.Revoke(storedToken);

            var newRefreshToken = new AuthToken
            {
                Id = Guid.NewGuid(),
                UserId = user.Id,
                Token = Guid.NewGuid().ToString("N"),
                CreatedAt = DateTime.UtcNow,
                ExpiresAt = DateTime.UtcNow.AddDays(7),
                IsRevoked = false
            };

            var accessToken = _jwtTokenService.GenerateToken(user);

            _authTokenRepository.Add(newRefreshToken);

            await _uow.SaveChangesAsync();
            await _uow.CommitAsync();

            return new RefreshTokenResponse(
                accessToken.AccessToken,
                accessToken.TokenType,
                accessToken.IssuedAt,
                accessToken.ExpiresAt,
                newRefreshToken.Token
            );
        }
        catch (Exception ex)
        {
            await _uow.RollbackAsync();
            _logService.ErrorLog($"{nameof(AuthService)}.{nameof(RefreshTokenAsync)}", ex);
            return null;
        }
    }

}
