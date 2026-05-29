
namespace FiscalCore.Domain.Interfaces.Stamping;

public interface IStampBalanceRepository
{
    Task<StampBalance?> GetByUserIdAsync(Guid userId, bool asTracking = true);
    void Add(StampBalance stampBalance);
    void Update(StampBalance stampBalance);
    void Delete(StampBalance stampBalance);
    void Decrement(Guid userId, int decrementStamps);

}

