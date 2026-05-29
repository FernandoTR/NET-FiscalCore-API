
namespace FiscalCore.Domain.Interfaces.Cfdis;

public interface ICfdiRepository
{
    Task<Cfdi?> GetByIdAsync(Guid id);
    Task<Cfdi?> GetByUuidAsync(Guid uuid);
    void Add(Cfdi cfdi);
    void Update(Cfdi cfdi);
    void Delete(Cfdi cfdi);
}
