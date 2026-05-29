
namespace FiscalCore.Domain.Interfaces.Certificates;

public interface ICertificateRepository
{
    Task<Certificate?> GetByIdAsync(Guid certificateId);
    Task<IReadOnlyList<Certificate>> GetByUserAsync(Guid userId);
    Task<Certificate?> GetActiveByRfcAsync(string rfc);
    void Add(Certificate certificate);
    void Update(Certificate certificate);
    void Delete(Certificate certificate);
}
