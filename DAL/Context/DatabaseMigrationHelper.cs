using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace DAL.Context
{
    public class DatabaseMigrationHelper
    {
        private readonly AppDbContext _context;
        private readonly ILogger<DatabaseMigrationHelper> _logger;

        public DatabaseMigrationHelper(AppDbContext context, ILogger<DatabaseMigrationHelper> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<bool> EnsureDatabaseCreatedAsync()
        {
            try
            {
                _logger.LogInformation("Ensuring database exists");
                
                var created = await _context.Database.EnsureCreatedAsync();
                
                if (created)
                {
                    _logger.LogInformation("Database created successfully");
                }
                else
                {
                    _logger.LogInformation("Database already exists");
                }
                
                return created;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating database");
                throw;
            }
        }

        public async Task ApplyMigrationsAsync()
        {
            try
            {
                var pendingMigrations = await _context.Database.GetPendingMigrationsAsync();
                
                if (pendingMigrations.Any())
                {
                    _logger.LogInformation($"Applying {pendingMigrations.Count()} pending migrations");
                    await _context.Database.MigrateAsync();
                    _logger.LogInformation("Migrations applied successfully");
                }
                else
                {
                    _logger.LogInformation("No pending migrations");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error applying migrations");
                throw;
            }
        }

        public async Task<string> GetDatabaseInfoAsync()
        {
            try
            {
                var providerName = _context.Database.ProviderName;
                var connectionString = _context.Database.GetConnectionString();
                
                return $"Provider: {providerName}, Connection: {connectionString}";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting database info");
                return $"Error: {ex.Message}";
            }
        }

        public async Task<bool> CanConnectAsync()
        {
            try
            {
                return await _context.Database.CanConnectAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Cannot connect to database");
                return false;
            }
        }
    }
} 