using FiscalCore.Application.Interfaces.Logging;
using FiscalCore.Infrastructure.Persistence.Context;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace FiscalCore.Infrastructure.Logging;

public class LogService : ILogService
{
    private readonly LoggingDbContext _loggingDbContext;

    public LogService(LoggingDbContext loggingDbContext)
    {
        _loggingDbContext = loggingDbContext;
    }

    /// <summary>
    /// Inserta logs de error en base de datos.
    /// </summary>
    /// <param name="methodName"></param>
    /// <param name="exception"></param>
    public void ErrorLog(string methodName, Exception exception)
    {
        try
        {
            var extras = string.Empty;

            if (exception is DbUpdateException dbUpdateException)
            {
                foreach (var entry in dbUpdateException.Entries)
                {
                    extras += $"Entity: {entry.Entity.GetType().Name}, State: {entry.State} ";
                }
            }
            else if (exception is ValidationException validationException)
            {
                extras = validationException.Message;
            }

            var log = new ErrorLog
            {
                MethodName = methodName,
                ExceptionMessage = exception.Message,
                ExceptionStackTrace = exception.StackTrace + ":" + extras,
                ExceptionString = exception.ToString(),
                LogDate = DateTime.UtcNow,
            };

            _loggingDbContext.ErrorLogs.Add(log);
            _loggingDbContext.SaveChanges();
        }
        catch
        {
            // Se sugiere loggear este error en un logger externo
        }
    }

    /// <summary>
    /// Inserta logs de error en base de datos sin ser producidos por exception.
    /// </summary>
    public void ErrorLog(string methodName, string message, string details)
    {
        try
        {
            var log = new ErrorLog
            {
                MethodName = methodName,
                ExceptionMessage = message,
                ExceptionStackTrace = "No se creó por excepción",
                ExceptionString = details,
                LogDate = DateTime.UtcNow
            };

            _loggingDbContext.ErrorLogs.Add(log);
            _loggingDbContext.SaveChanges();
        }
        catch
        {
            // Se sugiere loggear este error en un logger externo
        }
    }

    /// <summary>
    /// Inserta logs de actividad en base de datos.
    /// </summary>
    public void ActivityLog(Guid userId, string eventType, string description)
    {
        try
        {
            var log = new ActivityLog
            {
                LogDate = DateTime.UtcNow,
                EventType = eventType,
                Description = description,
                UserId = userId,
            };

            _loggingDbContext.ActivityLogs.Add(log);
            _loggingDbContext.SaveChanges();
        }
        catch (Exception ex)
        {
            ErrorLog("LogService.ActivityLog", ex);
            throw;
        }
    }

}
