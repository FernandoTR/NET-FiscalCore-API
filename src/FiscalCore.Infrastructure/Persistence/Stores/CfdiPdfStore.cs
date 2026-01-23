using FiscalCore.Application.DTOs.Cfdis;
using FiscalCore.Application.Interfaces.Cfdis;

namespace FiscalCore.Infrastructure.Persistence.Stores;

public sealed class CfdiPdfStore : ICfdiPdfStore
{
    private readonly FiscalCoreDbContext _context;

    public CfdiPdfStore(FiscalCoreDbContext context)
    {
        _context = context;
    }

    public async Task<CfdiPdfDto?> GetByIdAsync(Guid id)
    {
        var entity = await _context.CfdiPdfs
            .AsNoTracking()
            .FirstOrDefaultAsync(p => p.Id == id);

        return entity is null ? null : MapToDto(entity);
    }

    public async Task<CfdiPdfDto?> GetByCfdiIdAsync(Guid cfdiId)
    {
        var entity = await _context.CfdiPdfs
            .AsNoTracking()
            .FirstOrDefaultAsync(p => p.CfdiId == cfdiId);

        return entity is null ? null : MapToDto(entity);
    }

    public void Add(CfdiPdfDto dto)
    {
        var entity = MapToEntity(dto);
        _context.CfdiPdfs.Add(entity);
    }

    public void Update(CfdiPdfDto dto)
    {
        var entity = MapToEntity(dto);
        _context.CfdiPdfs.Update(entity);
    }

    public void Delete(CfdiPdfDto dto)
    {
        var entity = MapToEntity(dto);
        _context.CfdiPdfs.Remove(entity);
    }

    // -------------------------
    // Mapping 
    // -------------------------

    private static CfdiPdfDto MapToDto(CfdiPdf entity)
    {
        return new CfdiPdfDto
        {
            Id = entity.Id,
            CfdiId = entity.CfdiId,
            FilePath = entity.FilePath,
            Version = entity.Version,
            CreatedAt = entity.CreatedAt
        };
    }

    private static CfdiPdf MapToEntity(CfdiPdfDto dto)
    {
        return new CfdiPdf
        {
            Id = dto.Id,
            CfdiId = dto.CfdiId,
            FilePath = dto.FilePath,
            Version = dto.Version,
            CreatedAt = dto.CreatedAt
        };
    }
}
