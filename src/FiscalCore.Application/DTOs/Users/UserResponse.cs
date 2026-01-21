namespace FiscalCore.Application.DTOs.Users;

public record UserResponse(
    Guid Id,
    string Email,
    string Username,
    bool IsActive,
    DateTime CreatedAt
);