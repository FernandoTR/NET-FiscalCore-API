namespace FiscalCore.Application.DTOs.Certificate;

public class CertificateResponse
{
    public Guid CertificateId { get; set; }
    public Guid UserId { get; init; }
    public string Rfc { get; init; } = null!;
    public string SerialNumber { get; init; } = null!;
    public string CertificateType { get; init; } = null!;
    public DateOnly ValidFrom { get; init; }
    public DateOnly ValidTo { get; init; }
    public byte[] CerFile { get; init; } = null!;
    public byte[] KeyFile { get; init; } = null!;
    public string EncryptedKeyPassword { get; init; } = null!;
    public bool IsActive { get; set; }

}
