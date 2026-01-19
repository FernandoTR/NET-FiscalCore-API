
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FiscalCore.Infrastructure.Persistence.Configurations;

public class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.ToTable("Users");

        // Primary Key
        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id)
               .HasDefaultValueSql("(newid())");

        // Username
        builder.Property(x => x.Username)
               .HasMaxLength(100)
               .IsRequired();

        // Email
        builder.Property(x => x.Email)
               .HasMaxLength(150)
               .IsRequired();

        // Password
        builder.Property(x => x.PasswordHash)
               .IsRequired();

        // Estado
        builder.Property(x => x.IsActive)
               .IsRequired()
               .HasDefaultValue(true);

        // Fecha de creación
        builder.Property(x => x.CreatedAt)
               .HasDefaultValueSql("(sysdatetime())")
               .IsRequired();

        // Índices únicos
        builder.HasIndex(x => x.Username)
               .IsUnique()
               .HasDatabaseName("IX_Users_Username");

        builder.HasIndex(x => x.Email)
               .IsUnique()
               .HasDatabaseName("IX_Users_Email");
    }
}
