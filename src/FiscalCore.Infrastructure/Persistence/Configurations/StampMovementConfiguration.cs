using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FiscalCore.Infrastructure.Persistence.Configurations;

public class StampMovementConfiguration : IEntityTypeConfiguration<StampMovement>
{
    public void Configure(EntityTypeBuilder<StampMovement> builder)
    {
        // Nombre explícito de tabla 
        builder.ToTable("StampMovements");

        // Primary Key
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).HasDefaultValueSql("(newid())");

        // FK a StampBalance
        builder.Property(x => x.StampBalanceId).IsRequired();

        // Índice para consultas frecuentes
        builder.HasIndex(x => x.StampBalanceId)
               .HasDatabaseName("IX_StampMovements_StampBalanceId");        

        // Relación Many -> One
        builder.HasOne<StampBalance>()
               .WithMany()
               .HasForeignKey(x => x.StampBalanceId)
               .OnDelete(DeleteBehavior.Cascade);

        // FK a CFDI
        builder.Property(x => x.CfdiId).IsRequired();

        // Relación Many -> One
        builder.HasOne<Cfdi>()
               .WithMany()
               .HasForeignKey(x => x.CfdiId)
               .OnDelete(DeleteBehavior.Cascade);

        // Cantidad de timbres (+/-)
        builder.Property(x => x.Amount).IsRequired();

        // Fecha de creación
        builder.Property(x => x.CreatedAt)
               .HasDefaultValueSql("(sysdatetime())")
               .IsRequired();
        
    }
}
