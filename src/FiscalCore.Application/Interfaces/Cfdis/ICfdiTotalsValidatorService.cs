using FiscalCore.Application.DTOs.Cfdis;
using FiscalCore.Application.DTOs.Common;

namespace FiscalCore.Application.Interfaces.Cfdis;

public interface ICfdiTotalsValidatorService
{
    ResponseDto Execute(CreateCfdiRequest request);
}
