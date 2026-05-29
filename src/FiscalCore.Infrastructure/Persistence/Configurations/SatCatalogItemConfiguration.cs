
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FiscalCore.Infrastructure.Persistence.Configurations;

public class SatCatalogItemConfiguration : IEntityTypeConfiguration<SatCatalogItem>
{
    public void Configure(EntityTypeBuilder<SatCatalogItem> builder)
    {
        builder.ToTable("SatCatalogItems");

        // Primary Key
        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id)
               .HasDefaultValueSql("(newid())");

        // FK a SatCatalog
        builder.Property(x => x.SatCatalogId)
               .IsRequired();

        builder.HasIndex(x => x.SatCatalogId);

        builder.HasOne<SatCatalog>()
               .WithMany()
               .HasForeignKey(x => x.SatCatalogId)
               .OnDelete(DeleteBehavior.Cascade)
               .HasConstraintName("FK_SatCatalogItems_SatCatalogs");

        // Clave SAT (ej. 01, PUE, G01)
        builder.Property(x => x.KeyCode)
               .HasMaxLength(20)
               .IsRequired();

        // Descripción
        builder.Property(x => x.Description)
               .HasMaxLength(500)
               .IsRequired();

        // Vigencia
        builder.Property(x => x.StartDate)
               .IsRequired();

        builder.Property(x => x.EndDate)
               .IsRequired(false);

        // Estado
        builder.Property(x => x.IsActive)
               .IsRequired()
               .HasDefaultValue(true);

        // Fecha creación
        builder.Property(x => x.CreatedAt)
               .HasDefaultValueSql("(sysdatetime())")
               .IsRequired();

        // Índice único por catálogo + clave
        builder.HasIndex(x => new { x.SatCatalogId, x.KeyCode })
               .IsUnique()
               .HasDatabaseName("UX_SatCatalogItems_Catalog_Key");
    }
}