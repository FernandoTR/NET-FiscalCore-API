
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FiscalCore.Infrastructure.Persistence.Configurations;

public class CfdiXmlConfiguration : IEntityTypeConfiguration<CfdiXml>
{
    public void Configure(EntityTypeBuilder<CfdiXml> builder)
    {
        builder.ToTable("CfdiXmls");

        // Primary Key
        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id)
               .HasDefaultValueSql("(newid())");

        // FK a Cfdi
        builder.Property(x => x.CfdiId)
               .IsRequired();

        builder.HasIndex(x => x.CfdiId)
               .HasDatabaseName("IX_CfdiXmls_CfdiId");

        builder.HasOne<Cfdi>()
               .WithMany()
               .HasForeignKey(x => x.CfdiId)
               .OnDelete(DeleteBehavior.Cascade);

        // Contenido XML
        builder.Property(x => x.XmlContent)
               .HasColumnType("nvarchar(max)")
               .IsRequired();

        // Fecha de creación
        builder.Property(x => x.CreatedAt)
               .HasDefaultValueSql("(sysdatetime())")
               .IsRequired();
    }
}
