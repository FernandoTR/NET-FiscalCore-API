
namespace FiscalCore.Domain.Interfaces.Cfdis;

public interface ICfdiRepository
{
    Task<Cfdi?> GetByIdAsync(Guid id);
    void Add(Cfdi cfdi);
    void Update(Cfdi cfdi);
    void Delete(Cfdi cfdi);
}
