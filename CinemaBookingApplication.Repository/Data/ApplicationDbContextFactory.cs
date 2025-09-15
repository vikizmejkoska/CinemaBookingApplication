using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using System;
using System.IO;

namespace CinemaBookingApplication.Repository.Data
{
    public class ApplicationDbContextFactory : IDesignTimeDbContextFactory<ApplicationDbContext>
    {
        public ApplicationDbContext CreateDbContext(string[] args)
        {
            // Resolve the Web project directory (../CinemaBookingApplication.Web relative to Repository)
            var webProjectPath = Path.GetFullPath(Path.Combine(
                Directory.GetCurrentDirectory(), "..", "CinemaBookingApplication.Web"));

            // Determine environment (optional, defaults to Development)
            var env = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Development";

            // Build configuration from Web/appsettings + env + environment variables
            var config = new ConfigurationBuilder()
                .SetBasePath(webProjectPath)
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: false)
                .AddJsonFile($"appsettings.{env}.json", optional: true, reloadOnChange: false)
                .AddEnvironmentVariables() // supports ConnectionStrings__DefaultConnection
                .Build();

            // Try to get the connection string
            var conn = config.GetConnectionString("DefaultConnection");

            // Fallback to env var if still null/empty
            if (string.IsNullOrWhiteSpace(conn))
                conn = Environment.GetEnvironmentVariable("ConnectionStrings__DefaultConnection");

            // Final fallback (keep something that works locally if you want)
            if (string.IsNullOrWhiteSpace(conn))
                conn = "Server=(localdb)\\mssqllocaldb;Database=CinemaBooking;Trusted_Connection=True;MultipleActiveResultSets=true";

            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseSqlServer(conn)
                .Options;

            return new ApplicationDbContext(options);
        }
    }
}
