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
            var connectionString = configuration.GetConnectionString("DefaultConnection")
                                  ?? "Server=KIETTA\\SQLEXPRESS;Database=VNS_Travel;Trusted_Connection=True;TrustServerCertificate=True;";

            var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>();
            optionsBuilder.UseSqlServer(connectionString);

            return new AppDbContext(optionsBuilder.Options);
        }
    }
}
