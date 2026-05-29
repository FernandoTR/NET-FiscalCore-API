
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FiscalCore.Infrastructure.Persistence.Configurations;

public class EmailLogConfiguration : IEntityTypeConfiguration<EmailLog>
{
    public void Configure(EntityTypeBuilder<EmailLog> builder)
    {
        builder.ToTable("EmailLogs");

        // Primary Key
        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id)
               .HasDefaultValueSql("(newid())");

        // FK a Cfdi
        builder.Property(x => x.CfdiId)
               .IsRequired();

        builder.HasIndex(x => x.CfdiId)
               .HasDatabaseName("IX_EmailLogs_CfdiId");

        builder.HasOne<Cfdi>()
               .WithMany()
               .HasForeignKey(x => x.CfdiId)
               .OnDelete(DeleteBehavior.Cascade);

        // Destinatario
        builder.Property(x => x.Recipient)
               .HasMaxLength(320)
               .IsRequired();

        // Fecha de envío
        builder.Property(x => x.SentAt)
               .HasDefaultValueSql("(sysdatetime())")
               .IsRequired();
    }
}

