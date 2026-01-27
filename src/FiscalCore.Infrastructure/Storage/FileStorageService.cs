
using FiscalCore.Application.Interfaces.FileStorage;
using Microsoft.Extensions.Configuration;

namespace FiscalCore.Infrastructure.Storage;

public sealed class FileStorageService : IFileStorageService
{
    private readonly string _basePath;

    public FileStorageService(IConfiguration configuration)
    {
        _basePath = configuration["Storage:BasePath"]
                    ?? throw new InvalidOperationException("Storage base path not configured.");
    }

    public async Task<string> SaveAsync(
        byte[] content,
        string fileName,
        string folder,
        CancellationToken ct)
    {
        var directoryPath = Path.Combine(_basePath, folder);

        if (!Directory.Exists(directoryPath))
            Directory.CreateDirectory(directoryPath);

        var filePath = Path.Combine(directoryPath, fileName);

        await File.WriteAllBytesAsync(filePath, content, ct);

        return filePath;
    }
}
