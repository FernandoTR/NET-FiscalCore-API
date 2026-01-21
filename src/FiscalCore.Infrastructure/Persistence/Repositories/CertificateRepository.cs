using FiscalCore.Domain.Interfaces.Certificates;

namespace FiscalCore.Infrastructure.Persistence.Repositories;

public sealed class CertificateRepository : ICertificateRepository
{
    private readonly FiscalCoreDbContext _context;

    public CertificateRepository(FiscalCoreDbContext context)
    {
        _context = context;
    }

    public async Task<Certificate?> GetByIdAsync(Guid certificateId)
    {
        return await _context.Certificates
            .AsNoTracking()
            .FirstOrDefaultAsync(c => c.CertificateId == certificateId);
    }

    public async Task<IReadOnlyList<Certificate>> GetByUserAsync(Guid userId)
    {
        return await _context.Certificates
            .AsNoTracking()
            .Where(c => c.UserId == userId)
            .OrderByDescending(c => c.CreatedAt)
            .ToListAsync();
    }

    public async Task<Certificate?> GetActiveByRfcAsync(string rfc)
    {
        return await _context.Certificates
            .AsNoTracking()
            .FirstOrDefaultAsync(c =>
                c.Rfc == rfc &&
                c.IsActive &&
                c.ValidFrom <= DateOnly.FromDateTime(DateTime.UtcNow) &&
                c.ValidTo >= DateOnly.FromDateTime(DateTime.UtcNow));
    }

    public void Add(Certificate certificate)
    {
        _context.Certificates.Add(certificate);
    }

    public void Update(Certificate certificate)
    {
        _context.Certificates.Update(certificate);
    }

    public void Delete(Certificate certificate)
    {
        _context.Certificates.Remove(certificate);
    }
}
