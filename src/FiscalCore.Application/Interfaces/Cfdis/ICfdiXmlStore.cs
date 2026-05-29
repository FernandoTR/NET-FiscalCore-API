using FiscalCore.Application.DTOs.Cfdis;

namespace FiscalCore.Application.Interfaces.Cfdis;

public interface ICfdiXmlStore
{
    Task<CfdiXmlDto?> GetByIdAsync(Guid id);
    Task<CfdiXmlDto?> GetByCfdiIdAsync(Guid cfdiId);
    void Add(CfdiXmlDto cfdiXml);
    void Update(CfdiXmlDto cfdiXml);
    void Delete(CfdiXmlDto cfdiXml);
}
