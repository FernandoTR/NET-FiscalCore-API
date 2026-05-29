using FiscalCore.Application.DTOs.Email;
using FiscalCore.Application.Interfaces.Email;
using Microsoft.Extensions.Configuration;
using System.Net;
using System.Net.Mail;
using System.Text;

namespace FiscalCore.Infrastructure.Email;

public sealed class SmtpEmailService : IEmailService
{
    private readonly EmailOptions _options;

    public SmtpEmailService(IConfiguration configuration)
    {
        _options = configuration
            .GetSection("Email")
            .Get<EmailOptions>()
            ?? throw new InvalidOperationException(
                "Email configuration section is missing");
    }

    public async Task SendAsync(
        string to,
        string subject,
        string body,
        IReadOnlyList<EmailAttachment> attachments,
        CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(to))
            throw new ArgumentException("Recipient is required", nameof(to));

        using var message = CreateMessage(to, subject, body, attachments);
        using var client = CreateClient();

        ct.ThrowIfCancellationRequested();

        await client.SendMailAsync(message);
    }

    private MailMessage CreateMessage(
        string to,
        string subject,
        string body,
        IReadOnlyList<EmailAttachment> attachments)
    {
        var message = new MailMessage
        {
            From = new MailAddress(_options.From),
            Subject = subject,
            Body = body,
            IsBodyHtml = true,
            BodyEncoding = Encoding.UTF8,
            SubjectEncoding = Encoding.UTF8
        };

        message.To.Add(to);

        foreach (var attachment in attachments)
        {
            var stream = new MemoryStream(attachment.Content, writable: false);

            var mailAttachment = new Attachment(
                stream,
                attachment.FileName,
                attachment.ContentType);

            message.Attachments.Add(mailAttachment);
        }

        return message;
    }

    private SmtpClient CreateClient()
    {
        return new SmtpClient(_options.Smtp.Host, _options.Smtp.Port)
        {
            EnableSsl = _options.Smtp.EnableSsl,
            UseDefaultCredentials = false,
            Credentials = new NetworkCredential(
                _options.Smtp.User,
                _options.Smtp.Password),
            Timeout = _options.Smtp.TimeoutMs
        };
    }
}

