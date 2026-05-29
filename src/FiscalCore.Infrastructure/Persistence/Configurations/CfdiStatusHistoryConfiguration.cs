
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FiscalCore.Infrastructure.Persistence.Configurations;

public class CfdiStatusHistoryConfiguration : IEntityTypeConfiguration<CfdiStatusHistory>
{
    public void Configure(EntityTypeBuilder<CfdiStatusHistory> builder)
    {
        builder.ToTable("CfdiStatusHistories");

        // Primary Key
        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id)
               .HasDefaultValueSql("(newid())");

        // FK a Cfdi
        builder.Property(x => x.CfdiId)
               .IsRequired();

        builder.HasIndex(x => x.CfdiId)
               .HasDatabaseName("IX_CfdiStatusHistories_CfdiId");

        builder.HasOne<Cfdi>()
               .WithMany()
               .HasForeignKey(x => x.CfdiId)
               .OnDelete(DeleteBehavior.Cascade);

        // Estado del CFDI
        builder.Property(x => x.Status)
               .HasMaxLength(50)
               .IsRequired();

        // Fecha del cambio de estado
        builder.Property(x => x.ChangedAt)
               .HasDefaultValueSql("(sysdatetime())")
               .IsRequired();

        // Índice compuesto para historial ordenado
        builder.HasIndex(x => new { x.CfdiId, x.ChangedAt })
               .HasDatabaseName("IX_CfdiStatusHistories_CfdiId_ChangedAt");
    }
}
