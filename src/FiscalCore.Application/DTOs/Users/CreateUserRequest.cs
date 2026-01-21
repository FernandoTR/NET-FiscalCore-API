namespace FiscalCore.Application.DTOs.Users;

public sealed class CreateUserRequest
{
    public string Username { get; set; } = null!;

    public string PasswordHash { get; set; } = null!;

    public string Email { get; set; } = null!;
}
