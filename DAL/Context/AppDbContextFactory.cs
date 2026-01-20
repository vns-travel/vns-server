using DAL.Commons;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace DAL.Context
{
    /// <summary>
    /// Design-time factory to ensure EF tools use the intended provider.
    /// </summary>
    public class AppDbContextFactory : IDesignTimeDbContextFactory<AppDbContext>
    {
        public AppDbContext CreateDbContext(string[] args)
        {
            var configBuilder = new ConfigurationBuilder()
                .AddEnvironmentVariables();

            var presentationPath = Path.Combine(Directory.GetCurrentDirectory(), "Presentation");
            if (Directory.Exists(presentationPath))
            {
                configBuilder
                    .SetBasePath(presentationPath)
                    .AddJsonFile("appsettings.json", optional: true, reloadOnChange: false)
                    .AddJsonFile("appsettings.Development.json", optional: true, reloadOnChange: false);
            }

            var configuration = configBuilder.Build();
            var databaseTypeValue = configuration.GetValue<string>("Database:Type") ?? "sqlite";
            var connectionString = configuration.GetConnectionString("DefaultConnection")
                                  ?? configuration.GetConnectionString("SqliteConnection")
                                  ?? "Data Source=VNS_Travel.db";

            var databaseType = databaseTypeValue.ToLower() switch
            {
                "sqlserver" => DAL.Commons.DatabaseType.SqlServer,
                "sqlite" => DAL.Commons.DatabaseType.Sqlite,
                _ => DAL.Commons.DatabaseType.Sqlite
            };

            var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>();
            var databaseConfig = new DatabaseConfig
            {
                DatabaseType = databaseType,
                ConnectionString = connectionString
            };
            databaseConfig.ConfigureDbContext(optionsBuilder);

            return new AppDbContext(optionsBuilder.Options, databaseType);
        }
    }
}
