using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using RangShala.Data;

namespace RangShala.Data
{
    public class ApplicationDbContextFactory : IDesignTimeDbContextFactory<ApplicationDbContext>
    {
        public ApplicationDbContext CreateDbContext(string[] args)
        {
            Console.WriteLine($"Current Directory: {Directory.GetCurrentDirectory()}");
            var configPath = Path.Combine(Directory.GetCurrentDirectory(), "appsettings.json");
            Console.WriteLine($"Looking for appsettings.json at: {configPath}");
            Console.WriteLine($"File Exists: {File.Exists(configPath)}");

            try
            {
                var configuration = new ConfigurationBuilder()
                    .SetBasePath(Directory.GetCurrentDirectory())
                    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                    .Build();

                Console.WriteLine("Configuration loaded successfully.");
                var connectionString = configuration.GetConnectionString("DefaultConnection");
                Console.WriteLine($"Connection String: {connectionString}");

                if (string.IsNullOrEmpty(connectionString))
                {
                    throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
                }

                var builder = new DbContextOptionsBuilder<ApplicationDbContext>();
                builder.UseSqlServer(connectionString);
                return new ApplicationDbContext(builder.Options);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
                Console.WriteLine($"Stack Trace: {ex.StackTrace}");
                throw;
            }
        }
    }
}