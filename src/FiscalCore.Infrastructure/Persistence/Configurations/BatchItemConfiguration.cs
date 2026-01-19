using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FiscalCore.Infrastructure.Persistence.Configurations;

public class BatchItemConfiguration : IEntityTypeConfiguration<BatchItem>
{
    public void Configure(EntityTypeBuilder<BatchItem> builder)
    {
        builder.ToTable("BatchItems");

        // Primary Key
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).HasDefaultValueSql("(newid())");

        // FK a CfdiBatch
        builder.Property(x => x.BatchId).IsRequired();

        // Relación Many -> One
        builder.HasOne<CfdiBatch>()
               .WithMany()
               .HasForeignKey(x => x.BatchId)
               .OnDelete(DeleteBehavior.Cascade);

        // FK a CFDI
        builder.Property(x => x.CfdiId).IsRequired();

        // Relación Many -> One
        builder.HasOne<Cfdi>()
               .WithMany()
               .HasForeignKey(x => x.CfdiId)
               .OnDelete(DeleteBehavior.Cascade);

        builder.Property(x => x.Status).HasMaxLength(30).IsRequired();

    }
}
