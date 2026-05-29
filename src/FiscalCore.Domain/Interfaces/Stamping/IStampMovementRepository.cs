namespace FiscalCore.Domain.Interfaces.Stamping;

public interface IStampMovementRepository
{
    Task<StampMovement?> GetByIdAsync(Guid id);
    Task<StampMovement?> GetByStampBalanceIdAsync(Guid stampBalanceId);
    Task<StampMovement?> GetByCfdiIdAsync(Guid cfdiId);
    void Add(StampMovement stampMovement);
    void Update(StampMovement stampMovement);
    void Delete(StampMovement stampMovement);
}
