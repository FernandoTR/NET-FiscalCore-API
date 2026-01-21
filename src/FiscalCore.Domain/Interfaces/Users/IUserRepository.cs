using FiscalCore.Domain.Entities;

namespace FiscalCore.Domain.Interfaces.Users;

public interface IUserRepository
{
    Task<User?> GetByIdAsync(Guid id);
    Task<User?> GetByEmailAsync(string email);
    Task<User?> GetByUsernameAsync(string username);

    Task<bool> EmailExistsAsync(string email, Guid? excludeUserId = null);
    Task<bool> UsernameExistsAsync(string username, Guid? excludeUserId = null);
    Task<bool> ExistsByEmailAsync(string email);

    void Add(User user);
    void Update(User user);   
    void Remove(User user);
}
