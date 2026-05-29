
using FiscalCore.Application.DTOs.Common;

namespace FiscalCore.Application.Interfaces.Cfdis;

public interface ICfdiPdfGenerateOrRegenerateService
{
    Task<ResponseDto> ExecuteAsync(Guid uuid, CancellationToken cancellationToken);
}
