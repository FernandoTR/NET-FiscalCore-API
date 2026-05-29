namespace FiscalCore.Domain.Entities;

public partial class Certificate
{
    public Guid CertificateId { get; set; }

    public Guid UserId { get; set; }

    public string Rfc { get; set; } = null!;

    public string SerialNumber { get; set; } = null!;

    public string CertificateType { get; set; } = null!;

    public DateOnly ValidFrom { get; set; }

    public DateOnly ValidTo { get; set; }

    public byte[] CerFile { get; set; } = null!;

    public byte[] KeyFile { get; set; } = null!;

    public string EncryptedKeyPassword { get; set; } = null!;

    public bool IsActive { get; set; }

    public DateTime CreatedAt { get; set; }
   
}
