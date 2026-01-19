
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FiscalCore.Infrastructure.Persistence.Configurations;

public class SatCatalogRuleConfiguration : IEntityTypeConfiguration<SatCatalogRule>
{
    public void Configure(EntityTypeBuilder<SatCatalogRule> builder)
    {
        builder.ToTable("SatCatalogRules");

        // Primary Key
        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id)
               .HasDefaultValueSql("(newid())");

        // Catálogo origen
        builder.Property(x => x.CatalogCode)
               .HasMaxLength(50)
               .IsRequired();

        builder.Property(x => x.ItemKey)
               .HasMaxLength(20)
               .IsRequired();

        // Catálogo destino
        builder.Property(x => x.AppliesToCatalog)
               .HasMaxLength(50)
               .IsRequired();

        builder.Property(x => x.AppliesToKey)
               .HasMaxLength(20)
               .IsRequired();

        // Regla
        builder.Property(x => x.IsAllowed)
               .IsRequired();

        // Fecha de creación
        builder.Property(x => x.CreatedAt)
               .HasDefaultValueSql("(sysdatetime())")
               .IsRequired();

        // Índice compuesto para validaciones SAT
        builder.HasIndex(x => new
        {
            x.CatalogCode,
            x.ItemKey,
            x.AppliesToCatalog,
            x.AppliesToKey
        })
        .HasDatabaseName("IX_SatCatalogRules_Validation");
    }
}
