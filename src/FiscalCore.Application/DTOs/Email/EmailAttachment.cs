namespace FiscalCore.Application.DTOs.Email;

public sealed class EmailAttachment
{
    public string FileName { get; }
    public string ContentType { get; }
    public byte[] Content { get; }

    private EmailAttachment(string fileName, byte[] content, string contentType)
    {
        FileName = fileName;
        Content = content;
        ContentType = contentType;
    }

    // -------------------------
    // Factory methods
    // -------------------------

    public static EmailAttachment FromBytes(string fileName, byte[] content, string contentType = "application/octet-stream")
    {
        if (string.IsNullOrWhiteSpace(fileName))
            throw new ArgumentException("FileName is required", nameof(fileName));

        if (content is null || content.Length == 0)
            throw new ArgumentException("Attachment content cannot be empty", nameof(content));

        return new EmailAttachment(fileName, content, contentType);
    }

    public static EmailAttachment FromFile(string filePath, string? fileName = null, string contentType = "application/octet-stream")
    {
        if (string.IsNullOrWhiteSpace(filePath))
            throw new ArgumentException("FilePath is required", nameof(filePath));

        if (!File.Exists(filePath))
            throw new FileNotFoundException("Attachment file not found", filePath);

        var content = File.ReadAllBytes(filePath);
        var resolvedFileName = fileName ?? Path.GetFileName(filePath);

        return new EmailAttachment(resolvedFileName, content, contentType);
    }
}
