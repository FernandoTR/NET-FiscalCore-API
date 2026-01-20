using FiscalCore.Domain.Entities;

namespace FiscalCore.Domain.Interfaces.Users;

public interface IUserRepository
{
    Task<User?> GetByIdAsync(Guid id);
    Task<User?> GetByEmailAsync(string email);

    void Add(User user);
    void Update(User user);
    Task<bool> ExistsByEmailAsync(string email);
}
