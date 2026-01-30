using FiscalCore.Application.DTOs.Cfdis;
using FiscalCore.Application.DTOs.Common;

namespace FiscalCore.Application.Interfaces.Cfdis;

public interface ICfdiEmailResendService
{
    Task<ResponseDto> ResendAsync(CfdiEmailResendRequest request, CancellationToken ct);
}
