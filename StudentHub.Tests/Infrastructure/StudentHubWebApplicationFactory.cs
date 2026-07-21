using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using StudentHub.API.Context;

namespace StudentHub.Tests.Infrastructure;

public class StudentHubWebApplicationFactory : WebApplicationFactory<Program>, IDisposable
{
    private readonly SqliteConnection _connection;

    static StudentHubWebApplicationFactory()
    {
        Environment.SetEnvironmentVariable("DISCORD_ID", "test-discord-client-id");
        Environment.SetEnvironmentVariable("DISCORD_SECRET", "test-discord-client-secret");
        Environment.SetEnvironmentVariable("JWT_SECRET", "test-jwt-secret-key-32-chars-min!!");
    }

    public StudentHubWebApplicationFactory()
    {
        _connection = new SqliteConnection("Data Source=:memory:");
        _connection.Open();
    }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Testing");

        builder.ConfigureServices(services =>
        {
            var descriptors = services
                .Where(d =>
                    d.ServiceType == typeof(DbContextOptions<StudentHubDbContext>) ||
                    d.ServiceType == typeof(StudentHubDbContext))
                .ToList();

            foreach (var descriptor in descriptors)
            {
                services.Remove(descriptor);
            }

            services.AddDbContext<StudentHubDbContext>(options =>
            {
                options.UseSqlite(_connection);
            });
        });
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            _connection.Dispose();
        }

        base.Dispose(disposing);
    }
}
