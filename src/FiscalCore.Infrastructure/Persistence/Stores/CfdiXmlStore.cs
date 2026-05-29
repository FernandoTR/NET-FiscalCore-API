
using FiscalCore.Application.DTOs.Cfdis;
using FiscalCore.Application.Interfaces.Cfdis;

namespace FiscalCore.Infrastructure.Persistence.Stores;

public class CfdiXmlStore : ICfdiXmlStore
{
    private readonly FiscalCoreDbContext _context;
    public CfdiXmlStore(FiscalCoreDbContext context)
    {
        _context = context;
    }

    public async Task<CfdiXmlDto?> GetByIdAsync(Guid id)
    {
        var entity = await _context.CfdiXmls
            .AsNoTracking()
            .FirstOrDefaultAsync(c => c.Id == id);

        return entity is null ? null : MapToDto(entity);
    }

    public async Task<CfdiXmlDto?> GetByCfdiIdAsync(Guid cfdiId)
    {
        var entity = await _context.CfdiXmls
           .AsNoTracking()
           .FirstOrDefaultAsync(c => c.CfdiId == cfdiId);

        return entity is null ? null : MapToDto(entity);
    }

    public void Add(CfdiXmlDto dto)
    {
        var entity = MapToEntity(dto);
        _context.CfdiXmls.Add(entity);
    }

    public void Update(CfdiXmlDto dto)
    {
        var entity = MapToEntity(dto);
        _context.CfdiXmls.Update(entity);
    }

    public void Delete(CfdiXmlDto dto)
    {
        var entity = MapToEntity(dto);
        _context.CfdiXmls.Remove(entity);
    }


    // -------------------------
    // Mapping
    // -------------------------

    private static CfdiXmlDto MapToDto(CfdiXml entity)
    {
        return new CfdiXmlDto
        {
            Id = entity.Id,
            CfdiId = entity.CfdiId,
            XmlContent = entity.XmlContent,
            CreatedAt = entity.CreatedAt
        };
    }

    private static CfdiXml MapToEntity(CfdiXmlDto dto)
    {
        return new CfdiXml
        {
            Id = dto.Id,
            CfdiId = dto.CfdiId,
            XmlContent = dto.XmlContent,
            CreatedAt = dto.CreatedAt
        };
    }




}
