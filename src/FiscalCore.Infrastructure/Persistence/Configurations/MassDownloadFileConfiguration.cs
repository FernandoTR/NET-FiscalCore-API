
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FiscalCore.Infrastructure.Persistence.Configurations;

public class MassDownloadFileConfiguration : IEntityTypeConfiguration<MassDownloadFile>
{
    public void Configure(EntityTypeBuilder<MassDownloadFile> builder)
    {
        builder.ToTable("MassDownloadFiles");

        // Primary Key
        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id)
               .HasDefaultValueSql("(newid())");

        // FK a MassDownloadRequest
        builder.Property(x => x.RequestId)
               .IsRequired();

        builder.HasIndex(x => x.RequestId)
               .HasDatabaseName("IX_MassDownloadFiles_RequestId");

        builder.HasOne<MassDownloadRequest>()
               .WithMany()
               .HasForeignKey(x => x.RequestId)
               .OnDelete(DeleteBehavior.Cascade);

        // UUID del CFDI
        builder.Property(x => x.Uuid)
               .IsRequired();

        builder.HasIndex(x => x.Uuid)
               .HasDatabaseName("IX_MassDownloadFiles_Uuid");

        // Ruta del archivo
        builder.Property(x => x.FilePath)
               .HasMaxLength(500)
               .IsRequired();
    }
}
