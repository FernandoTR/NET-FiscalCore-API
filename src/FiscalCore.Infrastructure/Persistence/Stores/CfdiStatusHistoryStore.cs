using FiscalCore.Application.DTOs.Cfdis;
using FiscalCore.Application.Interfaces.Cfdis;

namespace FiscalCore.Infrastructure.Persistence.Stores;

public sealed class CfdiStatusHistoryStore : ICfdiStatusHistoryStore
{
    private readonly FiscalCoreDbContext _context;

    public CfdiStatusHistoryStore(FiscalCoreDbContext context)
    {
        _context = context;
    }

    public async Task<CfdiStatusHistoryDto?> GetByIdAsync(Guid id)
    {
        var entity = await _context.CfdiStatusHistories
            .AsNoTracking()
            .FirstOrDefaultAsync(h => h.Id == id);

        return entity is null ? null : MapToDto(entity);
    }

    public async Task<IReadOnlyList<CfdiStatusHistoryDto>> GetByCfdiIdAsync(Guid cfdiId)
    {
        return await _context.CfdiStatusHistories
            .AsNoTracking()
            .Where(h => h.CfdiId == cfdiId)
            .OrderBy(h => h.ChangedAt)
            .Select(h => MapToDto(h))
            .ToListAsync();
    }

    public void Add(CfdiStatusHistoryDto dto)
    {
        var entity = MapToEntity(dto);
        _context.CfdiStatusHistories.Add(entity);
    }

    public void Delete(CfdiStatusHistoryDto dto)
    {
        var entity = MapToEntity(dto);
        _context.CfdiStatusHistories.Remove(entity);
    }

    // -------------------------
    // Mapping 
    // -------------------------

    private static CfdiStatusHistoryDto MapToDto(CfdiStatusHistory entity)
    {
        return new CfdiStatusHistoryDto
        {
            Id = entity.Id,
            CfdiId = entity.CfdiId,
            Status = entity.Status,
            ChangedAt = entity.ChangedAt
        };
    }

    private static CfdiStatusHistory MapToEntity(CfdiStatusHistoryDto dto)
    {
        return new CfdiStatusHistory
        {
            Id = dto.Id,
            CfdiId = dto.CfdiId,
            Status = dto.Status,
            ChangedAt = dto.ChangedAt
        };
    }
}
