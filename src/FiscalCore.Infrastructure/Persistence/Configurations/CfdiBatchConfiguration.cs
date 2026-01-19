
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FiscalCore.Infrastructure.Persistence.Configurations;

public class CfdiBatchConfiguration : IEntityTypeConfiguration<CfdiBatch>
{
    public void Configure(EntityTypeBuilder<CfdiBatch> builder)
    {
        builder.ToTable("CfdiBatches");

        // Primary Key
        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id)
               .HasDefaultValueSql("(newid())");

        // FK a User (sin navegación en dominio)
        builder.Property(x => x.UserId)
               .IsRequired();

        builder.HasIndex(x => x.UserId)
               .HasDatabaseName("IX_CfdiBatches_UserId");

        builder.HasOne<User>()
               .WithMany()
               .HasForeignKey(x => x.UserId)
               .OnDelete(DeleteBehavior.Restrict);

        // Estado del batch
        builder.Property(x => x.Status)
               .HasMaxLength(50)
               .IsRequired();

        // Fecha de creación
        builder.Property(x => x.CreatedAt)
               .HasDefaultValueSql("(sysdatetime())")
               .IsRequired();

        // Relación 1 -> N con BatchItem
        //builder.HasMany<BatchItem>()
        //       .WithOne()
        //       .HasForeignKey(x => x.BatchId)
        //       .OnDelete(DeleteBehavior.Cascade);
    }
}
