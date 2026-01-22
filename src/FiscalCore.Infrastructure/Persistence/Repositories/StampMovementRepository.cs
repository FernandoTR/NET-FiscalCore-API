using FiscalCore.Domain.Interfaces.Stamping;

namespace FiscalCore.Infrastructure.Persistence.Repositories;

public class StampMovementRepository : IStampMovementRepository
{
    private readonly FiscalCoreDbContext _context;

    public StampMovementRepository(FiscalCoreDbContext context)
    {
        _context = context;
    }

    public async Task<StampMovement?> GetByIdAsync(Guid id)
    {
        return await _context.StampMovements
            .AsNoTracking()
            .FirstOrDefaultAsync(c => c.Id == id);
    }

    public async Task<StampMovement?> GetByStampBalanceIdAsync(Guid stampBalanceId)
    {
        return await _context.StampMovements
            .AsNoTracking()
            .FirstOrDefaultAsync(c => c.StampBalanceId == stampBalanceId);
    }

    public async Task<StampMovement?> GetByCfdiIdAsync(Guid cfdiId)
    {
        return await _context.StampMovements
            .AsNoTracking()
            .FirstOrDefaultAsync(c => c.CfdiId == cfdiId);
    }

    public void Add(StampMovement stampMovement)
    {
        _context.StampMovements.Add(stampMovement);
    }

    public void Update(StampMovement stampMovement)
    {
        _context.StampMovements.Update(stampMovement);
    }

    public void Delete(StampMovement stampMovement)
    {
        _context.StampMovements.Remove(stampMovement);
    }


}
