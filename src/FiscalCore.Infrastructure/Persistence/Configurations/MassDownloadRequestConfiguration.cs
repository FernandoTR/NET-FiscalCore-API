

using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FiscalCore.Infrastructure.Persistence.Configurations;

public class MassDownloadRequestConfiguration : IEntityTypeConfiguration<MassDownloadRequest>
{
    public void Configure(EntityTypeBuilder<MassDownloadRequest> builder)
    {
        builder.ToTable("MassDownloadRequests");

        // Primary Key
        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id)
               .HasDefaultValueSql("(newid())");

        // FK a User
        builder.Property(x => x.UserId)
               .IsRequired();

        builder.HasIndex(x => x.UserId)
               .HasDatabaseName("IX_MassDownloadRequests_UserId");

        builder.HasOne<User>()
               .WithMany()
               .HasForeignKey(x => x.UserId)
               .OnDelete(DeleteBehavior.Restrict);

        // Periodo (DateOnly requiere conversión)
        builder.Property(x => x.PeriodStart)
               .HasConversion(
                   v => v.ToDateTime(TimeOnly.MinValue),
                   v => DateOnly.FromDateTime(v))
               .HasColumnType("date")
               .IsRequired();

        builder.Property(x => x.PeriodEnd)
               .HasConversion(
                   v => v.ToDateTime(TimeOnly.MinValue),
                   v => DateOnly.FromDateTime(v))
               .HasColumnType("date")
               .IsRequired();

        // Status
        builder.Property(x => x.Status)
               .HasMaxLength(50)
               .IsRequired();

        // Fecha de creación
        builder.Property(x => x.CreatedAt)
               .HasDefaultValueSql("(sysdatetime())")
               .IsRequired();

    }
}
