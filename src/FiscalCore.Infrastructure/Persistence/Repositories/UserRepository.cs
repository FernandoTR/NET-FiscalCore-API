using FiscalCore.Domain.Interfaces.Users;

namespace FiscalCore.Infrastructure.Persistence.Repositories;

public class UserRepository : IUserRepository
{
    private readonly FiscalCoreDbContext _context;

    public UserRepository(FiscalCoreDbContext context)
    {
        _context = context;
    }

    public async Task<User?> GetByIdAsync(Guid id)
    {
        return await _context.Users
            .AsNoTracking()
            .FirstOrDefaultAsync(u => u.Id == id);
    }

    public async Task<User?> GetByEmailAsync(string email)
    {
        return await _context.Users
            .AsNoTracking()
            .FirstOrDefaultAsync(u => u.Email == email);
    }

    public async Task<User?> GetByUsernameAsync(string username)
    {
        return await _context.Users
            .AsNoTracking()
            .FirstOrDefaultAsync(u => u.Username == username);
    }


    public async Task<bool> EmailExistsAsync(string email, Guid? excludeUserId)
        => await _context.Users.AnyAsync(
            x => x.Email == email &&
                 (!excludeUserId.HasValue || x.Id != excludeUserId));

    public async Task<bool> UsernameExistsAsync(string username, Guid? excludeUserId)
      => await _context.Users.AnyAsync(
          x => x.Username == username &&
               (!excludeUserId.HasValue || x.Id != excludeUserId));


    public async Task<bool> ExistsByEmailAsync(string email)    
        => await _context.Users.AnyAsync(u => u.Email == email); 


    public void Add(User user)
         => _context.Users.Add(user);


    public void Update(User user)
        => _context.Users.Update(user);

    public void Remove(User user)
       => _context.Users.Remove(user);
    
}

