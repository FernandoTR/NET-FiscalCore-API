
using FiscalCore.Application.DTOs.Certificate;
using FiscalCore.Application.DTOs.Common;

namespace FiscalCore.Application.Interfaces.Certificate;

public interface ICertificateService
{
    Task<CertificateResponse?> GetByIdAsync(Guid certificateId);
    Task<IReadOnlyList<CertificateResponse>> GetByUserAsync(Guid userId);
    Task<CertificateResponse?> GetActiveByRfcAsync(string rfc);
    Task<ResponseDto> CreateAsync(CreateCertificateRequest certificate);
    Task<ResponseDto> UpdateAsync(UpdateCertificateRequest certificate);
    Task<ResponseDto> DeleteAsync(Guid certificateId);
}

