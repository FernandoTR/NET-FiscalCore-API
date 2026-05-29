
namespace FiscalCore.Application.Interfaces.FileStorage;

public interface IFileStorageService
{
    Task<string> SaveAsync(
        byte[] content,
        string fileName,
        string folder,
        CancellationToken ct);

    Task<byte[]> ReadAsync(
       string fileName,
       string folder,
       CancellationToken ct);


}
