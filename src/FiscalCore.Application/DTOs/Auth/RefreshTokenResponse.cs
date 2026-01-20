namespace FiscalCore.Application.DTOs.Auth;

public sealed record RefreshTokenResponse(
     string AccessToken,
     string TokenType,
     DateTime IssuedAt,
     DateTime ExpiresAt,
     string RefreshToken
);
