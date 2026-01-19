using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FiscalCore.Infrastructure.Persistence.Configurations;

public class CfdiPdfConfiguration : IEntityTypeConfiguration<CfdiPdf>
{
    public void Configure(EntityTypeBuilder<CfdiPdf> builder)
    {
        builder.ToTable("CfdiPdfs");

        // Primary Key
        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id)
               .HasDefaultValueSql("(newid())");

        // FK a Cfdi
        builder.Property(x => x.CfdiId)
               .IsRequired();

        builder.HasIndex(x => x.CfdiId)
               .HasDatabaseName("IX_CfdiPdfs_CfdiId");

        // Relación Many -> One
        builder.HasOne<Cfdi>()
               .WithMany()
               .HasForeignKey(x => x.CfdiId)
               .OnDelete(DeleteBehavior.Cascade);

        // Ruta del archivo
        builder.Property(x => x.FilePath)
               .HasMaxLength(500)
               .IsRequired();

        // Versión del PDF
        builder.Property(x => x.Version)
               .IsRequired();

        // Evitar PDFs duplicados por versión
        builder.HasIndex(x => new { x.CfdiId, x.Version })
               .IsUnique()
               .HasDatabaseName("UX_CfdiPdfs_CfdiId_Version");

        // Fecha de creación
        builder.Property(x => x.CreatedAt)
               .HasDefaultValueSql("(sysdatetime())")
               .IsRequired();
    }
}
