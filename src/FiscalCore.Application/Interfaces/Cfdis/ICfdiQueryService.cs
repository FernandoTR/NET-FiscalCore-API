using FiscalCore.Application.DTOs.Common;

namespace FiscalCore.Application.Interfaces.Cfdis;

public interface ICfdiQueryService
{
    Task<ResponseDto?> GetByUuidAsync(Guid uuid);
}

