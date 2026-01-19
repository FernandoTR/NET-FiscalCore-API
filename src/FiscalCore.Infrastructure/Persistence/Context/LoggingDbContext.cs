using FiscalCore.Infrastructure.Persistence.Configurations;
using System;
using System.Collections.Generic;
using System.Text;

namespace FiscalCore.Infrastructure.Persistence.Context;

// DbContext para manejar logs de errores en la base de datos (esto te permite aislar las operaciones de logging del resto de la aplicación)
public class LoggingDbContext : DbContext
{
    public LoggingDbContext(DbContextOptions<LoggingDbContext> options)
        : base(options)
    {
    }

    public DbSet<ActivityLog> ActivityLogs { get; set; }
    public DbSet<ErrorLog> ErrorLogs { get; set; }


    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Configuraciones adicionales para la tabla de logs
        modelBuilder.ApplyConfiguration(new ErrorLogConfiguration());
        modelBuilder.ApplyConfiguration(new ActivityLogConfiguration());
    }
}
