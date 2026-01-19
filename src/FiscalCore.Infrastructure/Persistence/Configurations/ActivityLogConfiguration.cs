using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FiscalCore.Infrastructure.Persistence.Configurations;

public class ActivityLogConfiguration : IEntityTypeConfiguration<ActivityLog>
{
    public void Configure(EntityTypeBuilder<ActivityLog> builder)
    {
        builder.ToTable("ActivityLogs");

        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).ValueGeneratedOnAdd();

        builder.Property(e => e.LogDate)
               .HasColumnType("datetime")
               .HasDefaultValueSql("(sysdatetime())")
               .IsRequired();

        builder.Property(x => x.EventType).HasMaxLength(250).IsRequired();
        builder.Property(x => x.Description).IsRequired();

        // FK a User
        builder.Property(x => x.UserId).IsRequired();

        builder.HasIndex(x => x.UserId)
       .HasDatabaseName("IX_ActivityLogs_UserId");

        // Relación Many -> One
        builder.HasOne<User>()
               .WithMany()
               .HasForeignKey(x => x.UserId)
               .OnDelete(DeleteBehavior.Cascade)
               .HasConstraintName("FK_Logs_Users");

    }
}
