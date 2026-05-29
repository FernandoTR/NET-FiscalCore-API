namespace FiscalCore.Infrastructure.Persistence.Context;

public sealed class FiscalCoreDbContext : DbContext
{   
    public FiscalCoreDbContext(DbContextOptions<FiscalCoreDbContext> options)
       : base(options) { }


    // ===== Core =====
    public DbSet<User> Users => Set<User>();

    // ===== Seguridad =====
    public DbSet<AuthToken> AuthTokens => Set<AuthToken>();

    // ===== Auditoría / Logs =====
    public DbSet<ActivityLog> ActivityLogs => Set<ActivityLog>();
    public DbSet<ErrorLog> ErrorLogs => Set<ErrorLog>();
    public DbSet<EmailLog> EmailLogs => Set<EmailLog>();

    // ===== CFDI =====
    public DbSet<Cfdi> Cfdis => Set<Cfdi>();
    public DbSet<CfdiBatch> CfdiBatches => Set<CfdiBatch>();
    public DbSet<BatchItem> BatchItems => Set<BatchItem>();
    public DbSet<CfdiStatusHistory> CfdiStatusHistories => Set<CfdiStatusHistory>();
    public DbSet<CfdiPdf> CfdiPdfs => Set<CfdiPdf>();
    public DbSet<CfdiXml> CfdiXmls => Set<CfdiXml>();

    // ===== Certificados =====
    public DbSet<Certificate> Certificates => Set<Certificate>();

    // ===== Descargas masivas =====
    public DbSet<MassDownloadRequest> MassDownloadRequests => Set<MassDownloadRequest>();
    public DbSet<MassDownloadFile> MassDownloadFiles => Set<MassDownloadFile>();

    // ===== SAT Catálogos =====
    public DbSet<SatCatalog> SatCatalogs => Set<SatCatalog>();
    public DbSet<SatCatalogItem> SatCatalogItems => Set<SatCatalogItem>();
    public DbSet<SatCatalogRule> SatCatalogRules => Set<SatCatalogRule>();

    // ===== Timbres =====
    public DbSet<StampBalance> StampBalances => Set<StampBalance>();
    public DbSet<StampMovement> StampMovements => Set<StampMovement>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Aplica TODAS las IEntityTypeConfiguration automáticamente
        modelBuilder.ApplyConfigurationsFromAssembly(
            typeof(FiscalCoreDbContext).Assembly
        );

        base.OnModelCreating(modelBuilder);
    }

}
