namespace FiscalCore.Application.DTOs.Auth;

public record JwtTokenDto(
    string AccessToken,
    string TokenType,
    DateTime IssuedAt,
    DateTime ExpiresAt
);