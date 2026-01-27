using FiscalCore.Application.DTOs.Email;

namespace FiscalCore.Application.Interfaces.Email;

public interface IEmailService
{
    Task SendAsync(
        string to,
        string subject,
        string body,
        IReadOnlyList<EmailAttachment> attachments,
        CancellationToken ct);
}

