namespace FiscalCore.Application.Interfaces.Logging;

public interface ILogService
{
    void ErrorLog(string methodName, Exception exception);
    void ErrorLog(string methodName, string message, string details);
    void ActivityLog(Guid userId, string eventType, string description);
}

