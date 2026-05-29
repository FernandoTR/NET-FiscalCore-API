
using FiscalCore.Application.DTOs.Cfdis;
using FiscalCore.Application.DTOs.Common;

namespace FiscalCore.Application.Interfaces.Cfdis;

public interface ICfdiFiscalRulesValidatorService
{
    Task<ResponseDto> ExecuteAsync(CreateCfdiRequest request, CancellationToken ct);
}
