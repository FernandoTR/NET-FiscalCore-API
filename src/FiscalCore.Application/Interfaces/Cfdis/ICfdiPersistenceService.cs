using FiscalCore.Application.DTOs.Cfdis;
using FiscalCore.Application.DTOs.Common;
using FiscalCore.Application.DTOs.Pac;

namespace FiscalCore.Application.Interfaces.Cfdis;

public interface ICfdiPersistenceService
{
    Task<ResponseDto> ExecuteAsync(PacStampingDto result, CreateCfdiRequest createCfdiRequest, Guid userId, Guid stampBalanceId);
}
