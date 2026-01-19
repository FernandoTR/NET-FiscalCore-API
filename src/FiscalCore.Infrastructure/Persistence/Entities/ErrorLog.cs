namespace FiscalCore.Infrastructure.Persistence.Entities;

public partial class ErrorLog
{
    public long Id { get; set; }

    public DateTime LogDate { get; set; }

    public string MethodName { get; set; } = null!;

    public string? ExceptionMessage { get; set; }

    public string? ExceptionStackTrace { get; set; }

    public string? ExceptionString { get; set; }
}
