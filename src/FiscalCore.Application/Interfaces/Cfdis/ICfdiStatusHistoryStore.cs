using FiscalCore.Application.DTOs.Cfdis;

namespace FiscalCore.Application.Interfaces.Cfdis;

public interface ICfdiStatusHistoryStore
{
    Task<CfdiStatusHistoryDto?> GetByIdAsync(Guid id);
    Task<IReadOnlyList<CfdiStatusHistoryDto>> GetByCfdiIdAsync(Guid cfdiId);
    void Add(CfdiStatusHistoryDto history);
    void Delete(CfdiStatusHistoryDto history);
}
