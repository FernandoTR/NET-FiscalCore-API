namespace FiscalCore.Application.DTOs.Auth;

public sealed record LoginResponse(
     string AccessToken,
     string TokenType,
     DateTime IssuedAt,
     DateTime ExpiresAt,
     string RefreshToken
);
