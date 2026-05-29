namespace FiscalCore.Application.DTOs.Email;

public sealed class EmailOptions
{
    public string From { get; init; } = default!;
    public SmtpOptions Smtp { get; init; } = default!;
}
