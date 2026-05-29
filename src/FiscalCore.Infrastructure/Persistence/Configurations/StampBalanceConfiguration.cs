using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FiscalCore.Infrastructure.Persistence.Configurations;

public class StampBalanceConfiguration : IEntityTypeConfiguration<StampBalance>
{
    public void Configure(EntityTypeBuilder<StampBalance> builder)
    {
        builder.ToTable("StampBalances");

        // Primary Key
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).HasDefaultValueSql("(newid())");

        // Relación 1:1 con User
        builder.Property(x => x.UserId).IsRequired();
        builder.HasIndex(x => x.UserId).IsUnique();

        builder.HasOne<User>()               // ⬅ NO navegación en User
               .WithOne()                    // ⬅ NO navegación inversa
               .HasForeignKey<StampBalance>(x => x.UserId)
               .OnDelete(DeleteBehavior.Cascade);

        // Campos de negocio
        builder.Property(x => x.TotalStamps).IsRequired();
        builder.Property(x => x.UsedStamps).IsRequired();
        builder.Ignore(x => x.AvailableStamps);

        builder.Property(x => x.CreatedAt)
              .HasDefaultValueSql("(sysdatetime())")
              .IsRequired();

       
    }
}

