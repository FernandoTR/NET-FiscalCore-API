using FiscalCore.Domain.Interfaces.Stamping;

namespace FiscalCore.Infrastructure.Persistence.Repositories;

public class StampBalanceRepository : IStampBalanceRepository
{
    private readonly FiscalCoreDbContext _context;

    public StampBalanceRepository(FiscalCoreDbContext context)
    {
        _context = context;
    }

    public async Task<StampBalance?> GetByUserIdAsync(Guid userId, bool asTracking = true)
    {
        var query = _context.StampBalances.AsQueryable();

        if (!asTracking)
            query = query.AsNoTracking();

        return await query.FirstOrDefaultAsync(c => c.UserId == userId);
    }


    public void Add(StampBalance stampBalance)
    {
        _context.StampBalances.Add(stampBalance);
    }

    public void Update(StampBalance stampBalance)
    {
        _context.StampBalances.Update(stampBalance);
    }

    public void Delete(StampBalance stampBalance)
    {
        _context.StampBalances.Remove(stampBalance);
    }

    public void Decrement(Guid userId, int decrementStamps)
    {
        var entity = _context.StampBalances.FirstOrDefault(c => c.UserId == userId);
        if (entity == null)
            throw new InvalidOperationException("Saldo de timbres no encontrado para el usuario especificado.");

        if (entity.AvailableStamps <= 0)
            throw new InvalidOperationException("Saldo de timbres insuficiente");

        entity.UsedStamps += decrementStamps;
        _context.StampBalances.Update(entity);
    }

}
