namespace FiscalCore.Application.DTOs.Users;

public class UpdateUserRequest
{
    public Guid Id { get; set; }

    public string Username { get; set; } = null!;

    public string PasswordHash { get; set; } = null!;

    public string Email { get; set; } = null!;

    public bool IsActive { get; set; }

}
