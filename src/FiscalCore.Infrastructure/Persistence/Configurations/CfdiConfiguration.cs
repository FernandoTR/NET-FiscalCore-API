
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FiscalCore.Infrastructure.Persistence.Configurations;

public class CfdiConfiguration : IEntityTypeConfiguration<Cfdi>
{
    public void Configure(EntityTypeBuilder<Cfdi> builder)
    {
        builder.ToTable("Cfdis");

        // Primary Key
        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id)
               .HasDefaultValueSql("(newid())");

        // FK a User
        builder.Property(x => x.UserId)
               .IsRequired();

        builder.HasOne<User>()
               .WithMany()
               .HasForeignKey(x => x.UserId)
               .OnDelete(DeleteBehavior.Restrict);

        // Identificador fiscal SAT
        builder.Property(x => x.Uuid)
               .IsRequired();

        builder.HasIndex(x => x.Uuid)
               .IsUnique();

        // RFCs
        builder.Property(x => x.RfcEmisor)
               .HasMaxLength(13)
               .IsRequired();

        builder.Property(x => x.RfcReceptor)
               .HasMaxLength(13)
               .IsRequired();

        // Versión CFDI
        builder.Property(x => x.Version)
               .HasMaxLength(10)
               .IsRequired();

        // Total
        builder.Property(x => x.Total)
               .HasColumnType("decimal(18,2)")
               .IsRequired();

        // Moneda (ISO 4217)
        builder.Property(x => x.Currency)
               .HasMaxLength(3)
               .IsRequired();

        // Fechas y estado
        builder.Property(x => x.IssueDate)
               .IsRequired();

        builder.Property(x => x.CurrentStatus)
               .HasMaxLength(50)
               .IsRequired();

        builder.Property(x => x.CreatedAt)
               .HasDefaultValueSql("(sysdatetime())")
               .IsRequired();

        // Índices de consulta frecuente
        builder.HasIndex(x => x.UserId)
               .HasDatabaseName("IX_Cfdis_UserId");

        builder.HasIndex(x => new { x.RfcEmisor, x.RfcReceptor })
               .HasDatabaseName("IX_Cfdis_RfcEmisor_RfcReceptor");
    }
}
