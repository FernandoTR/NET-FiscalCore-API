using FiscalCore.Domain.Interfaces.Cfdis;

namespace FiscalCore.Infrastructure.Persistence.Repositories;

public class CfdiRepository : ICfdiRepository
{
    private readonly FiscalCoreDbContext _context;

    public CfdiRepository(FiscalCoreDbContext context)
    {
        _context = context;
    }

    public async Task<Cfdi?> GetByIdAsync(Guid id)
    {
        return await _context.Cfdis
            .AsNoTracking()
            .FirstOrDefaultAsync(c => c.Id == id);
    }

    public async Task<Cfdi?> GetByUuidAsync(Guid uuid)
    {
        return await _context.Cfdis
            .AsNoTracking()
            .FirstOrDefaultAsync(c => c.Uuid == uuid);
    }

    public void Add(Cfdi cfdi)
    {
        _context.Cfdis.Add(cfdi);
    }

    public void Update(Cfdi cfdi)
    {
        _context.Cfdis.Update(cfdi);
    }

    public void Delete(Cfdi cfdi)
    {
        _context.Cfdis.Remove(cfdi);
    }

}
