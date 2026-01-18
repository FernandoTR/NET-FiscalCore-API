
namespace FiscalCore.Infrastructure.Security.Encryption;

public sealed class EncryptionOptions
{
    public const string SectionName = "Encryption";

    public string AesKey { get; init; } = string.Empty;
    public string AesIv { get; init; } = string.Empty;
}

