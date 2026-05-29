using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace FiscalCore.Infrastructure.Persistence.Context;

/// <summary>
/// Provides a design-time factory for creating instances of the FiscalCoreDbContext for use with Entity Framework Core
/// tools.
/// </summary>
/// <remarks>This class is used by Entity Framework Core tooling to create a FiscalCoreDbContext instance at
/// design time, such as during migrations or scaffolding operations. It reads configuration from the appsettings.json
/// file in the current directory to configure the database connection.</remarks>
public class FiscalCoreDbContextFactory : IDesignTimeDbContextFactory<FiscalCoreDbContext>
{
    public FiscalCoreDbContext CreateDbContext(string[] args)
    {
        var currentPath = Path.GetFullPath(Path.Combine(Directory.GetCurrentDirectory(), @"..\FiscalCore.Api"));
        var config = new ConfigurationBuilder()
            .SetBasePath(currentPath)
            .AddJsonFile("appsettings.json")
            .Build();
        
        var optionsBuilder = new DbContextOptionsBuilder<FiscalCoreDbContext>();
        optionsBuilder.UseSqlServer(config.GetConnectionString("DefaultConnection"));


        return new FiscalCoreDbContext(optionsBuilder.Options);
    }
}
