
using FiscalCore.Application.DTOs.Cfdis;

namespace FiscalCore.Application.Interfaces.Cfdis;

public interface ICfdiPdfStore
{
    Task<CfdiPdfDto?> GetByIdAsync(Guid id);
    Task<CfdiPdfDto?> GetByCfdiIdAsync(Guid cfdiId);
    void Add(CfdiPdfDto cfdiXml);
    void Update(CfdiPdfDto cfdiXml);
    void Delete(CfdiPdfDto cfdiXml);
}
