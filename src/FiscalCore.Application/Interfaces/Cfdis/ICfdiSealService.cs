
using FiscalCore.Application.DTOs.Certificate;
using FiscalCore.Application.DTOs.Common;

namespace FiscalCore.Application.Interfaces.Cfdis;

public interface ICfdiSealService
{
    ResponseDto Seal(string cadenaOriginal, CertificateResponse csd);
    ResponseDto Certificate(CertificateResponse csd);
}
