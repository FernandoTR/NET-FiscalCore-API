
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FiscalCore.Infrastructure.Persistence.Configurations;

public class ErrorLogConfiguration : IEntityTypeConfiguration<ErrorLog>
{
    public void Configure(EntityTypeBuilder<ErrorLog> builder)
    {
        builder.ToTable("ErrorLogs");

        // Primary Key
        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id)
               .ValueGeneratedOnAdd();

        // Fecha del error
        builder.Property(x => x.LogDate)
               .HasColumnType("datetime")
               .HasDefaultValueSql("(sysdatetime())")
               .IsRequired();

        // Método donde ocurrió el error
        builder.Property(x => x.MethodName)
               .HasMaxLength(300)
               .IsRequired();

        // Información de la excepción
        builder.Property(x => x.ExceptionMessage)
               .HasColumnType("nvarchar(max)");

        builder.Property(x => x.ExceptionStackTrace)
               .HasColumnType("nvarchar(max)");

        builder.Property(x => x.ExceptionString)
               .HasColumnType("nvarchar(max)");

        // Índices útiles para diagnóstico
        builder.HasIndex(x => x.LogDate)
               .HasDatabaseName("IX_ErrorLogs_LogDate");

        builder.HasIndex(x => x.MethodName)
               .HasDatabaseName("IX_ErrorLogs_MethodName");
    }
}
