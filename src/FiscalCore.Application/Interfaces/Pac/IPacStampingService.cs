
using FiscalCore.Application.DTOs.Pac;

namespace FiscalCore.Application.Interfaces.Pac;

public interface IPacStampingService
{
    Task<PacStampingDto> StampingXmlAsync(string cfdiXml, CancellationToken ct);
}
