using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FiscalCore.Infrastructure.Persistence.Configurations;

public class CertificateConfiguration : IEntityTypeConfiguration<Certificate>
{
    public void Configure(EntityTypeBuilder<Certificate> builder)
    {
        builder.ToTable("Certificates");

        // Primary Key
        builder.HasKey(x => x.CertificateId);

        builder.Property(x => x.CertificateId)
               .HasDefaultValueSql("(newid())");

        // FK a User
        builder.Property(x => x.UserId)
               .IsRequired();

        builder.HasIndex(x => x.UserId)
               .HasDatabaseName("IX_Certificates_UserId");

        builder.HasOne<User>()
               .WithMany()
               .HasForeignKey(x => x.UserId)
               .OnDelete(DeleteBehavior.Restrict);

        // RFC
        builder.Property(x => x.Rfc)
               .HasMaxLength(13)
               .IsRequired();

        // Número de serie del certificado
        builder.Property(x => x.SerialNumber)
               .HasMaxLength(50)
               .IsRequired();

        builder.HasIndex(x => x.SerialNumber)
               .IsUnique();

        // Tipo de certificado (CSD, FIEL)
        builder.Property(x => x.CertificateType)
               .HasMaxLength(20)
               .IsRequired();

        // Vigencia (DateOnly → date)
        builder.Property(x => x.ValidFrom)
               .HasColumnType("date")
               .IsRequired();

        builder.Property(x => x.ValidTo)
               .HasColumnType("date")
               .IsRequired();

        // Archivos
        builder.Property(x => x.CerFile)
               .IsRequired();

        builder.Property(x => x.KeyFile)
               .IsRequired();

        // Password cifrado
        builder.Property(x => x.EncryptedKeyPassword)
               .HasMaxLength(500)
               .IsRequired();

        // Estado
        builder.Property(x => x.IsActive)
               .IsRequired()
               .HasDefaultValue(true);

        // Fecha de creación
        builder.Property(x => x.CreatedAt)
               .HasDefaultValueSql("(sysdatetime())")
               .IsRequired();
    }
}
