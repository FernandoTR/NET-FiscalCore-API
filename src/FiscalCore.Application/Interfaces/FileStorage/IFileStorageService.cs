
namespace FiscalCore.Application.Interfaces.FileStorage;

public interface IFileStorageService
{
    Task<string> SaveAsync(
        byte[] content,
        string fileName,
        string folder,
        CancellationToken ct);
}
