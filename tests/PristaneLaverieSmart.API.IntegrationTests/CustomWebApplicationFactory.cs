using System.Linq;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Data.Sqlite;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using PristaneLaverieSmart.Infrastructure.Persistence.DbContext;

public sealed class CustomWebApplicationFactory: WebApplicationFactory<Program>
{
    private SqliteConnection? _connection;

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
    
        builder.ConfigureServices(services =>
        {
            // Remove existing DbContextOptions registration
            services.RemoveAll(typeof(DbContextOptions<PristaneLaverieSmartDbContext>));

            // Use shared in-memory SQLite connection
            _connection = new SqliteConnection("DataSource=:memory:");
            _connection.Open();

            services.AddDbContext<PristaneLaverieSmartDbContext>(options =>
                options.UseSqlite(_connection));
            
            // Build + create schema
            var sp = services.BuildServiceProvider();
            using var scope = sp.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<PristaneLaverieSmartDbContext>();
            db.Database.EnsureCreated();
            
        });
    }

    protected override void Dispose(bool disposing)
    {
        base.Dispose(disposing);
        _connection?.Dispose();
    }

}