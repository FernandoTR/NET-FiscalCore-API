namespace FiscalCore.Application.DTOs.Email;

public sealed class SmtpOptions
{
    public string Host { get; init; } = default!;
    public int Port { get; init; }
    public bool EnableSsl { get; init; } = true;
    public string User { get; init; } = default!;
    public string Password { get; init; } = default!;
    public int TimeoutMs { get; init; } = 10000;
}
