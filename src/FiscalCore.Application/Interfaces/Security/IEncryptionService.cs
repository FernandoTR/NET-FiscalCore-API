

namespace FiscalCore.Application.Interfaces.Security;

public interface IEncryptionService
{
    string Encrypt(string plainText);
    string Decrypt(string cipherText);
    byte[] EncryptByte(ReadOnlySpan<byte> data);
    byte[] DecryptByte(ReadOnlySpan<byte> data);
}
