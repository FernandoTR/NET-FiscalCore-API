using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FiscalCore.Infrastructure.Persistence.Configurations;

public class AuthTokenConfiguration : IEntityTypeConfiguration<AuthToken>
{
    public void Configure(EntityTypeBuilder<AuthToken> builder)
    {
        builder.ToTable("AuthTokens");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id)
               .HasDefaultValueSql("(newid())");

        // FK a User
        builder.Property(x => x.UserId)
               .IsRequired();

        builder.HasOne<User>()
               .WithMany()
               .HasForeignKey(x => x.UserId)
               .OnDelete(DeleteBehavior.Restrict);

        builder.Property(x => x.Token)
               .HasMaxLength(500)
               .IsRequired();

        builder.HasIndex(x => x.Token)
               .IsUnique();

        builder.Property(x => x.ExpiresAt)
               .IsRequired(false);

        builder.Property(x => x.CreatedAt)
               .HasDefaultValueSql("(sysdatetime())")
               .IsRequired();

        builder.Property(x => x.IsRevoked)
               .IsRequired()
               .HasDefaultValue(false);

    }
}
