
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FiscalCore.Infrastructure.Persistence.Configurations;

public class SatCatalogConfiguration : IEntityTypeConfiguration<SatCatalog>
{
    public void Configure(EntityTypeBuilder<SatCatalog> builder)
    {
        builder.ToTable("SatCatalogs");

        // Primary Key
        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id)
               .HasDefaultValueSql("(newid())");

        // Código SAT (ej. c_FormaPago)
        builder.Property(x => x.Code)
               .HasMaxLength(50)
               .IsRequired();

        builder.HasIndex(x => x.Code)
               .IsUnique()
               .HasDatabaseName("IX_SatCatalogs_Code");

        // Nombre del catálogo
        builder.Property(x => x.Name)
               .HasMaxLength(200)
               .IsRequired();

        // Descripción opcional
        builder.Property(x => x.Description)
               .HasMaxLength(500);

        // Versión CFDI (4.0)
        builder.Property(x => x.CfdiVersion)
               .HasMaxLength(10)
               .IsRequired();

        builder.HasIndex(x => x.CfdiVersion)
               .HasDatabaseName("IX_SatCatalogs_CfdiVersion");

        // Estado
        builder.Property(x => x.IsActive)
               .IsRequired()
               .HasDefaultValue(true);

        // Fecha de creación
        builder.Property(x => x.CreatedAt)
               .HasDefaultValueSql("(sysdatetime())")
               .IsRequired();

        // Relación 1 -> N con Items
        //builder.HasMany(x => x.Items)
        //       .WithOne(x => x.Catalog)
        //       .HasForeignKey(x => x.SatCatalogId)
        //       .OnDelete(DeleteBehavior.Cascade);
    }
}
