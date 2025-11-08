using AscendID_API.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace AscendID_API.WebAPI.Configuration;

public static class DatabaseConfiguration
{
    /// <summary>
    /// Configures and applies database migrations on application startup
    /// </summary>
    /// <param name="app">The web application</param>
    /// <returns>A task representing the asynchronous operation</returns>
    public static async Task ConfigureDatabaseAsync(this WebApplication app)
    {
        using var scope = app.Services.CreateScope();
        
        await ApplyDatabaseMigrationsAsync(scope.ServiceProvider);
    }

    /// <summary>
    /// Applies pending database migrations automatically on startup
    /// </summary>
    /// <param name="serviceProvider">The service provider</param>
    /// <returns>A task representing the asynchronous operation</returns>
    private static async Task ApplyDatabaseMigrationsAsync(IServiceProvider serviceProvider)
    {
        var logger = serviceProvider.GetRequiredService<ILogger<Program>>();
        var context = serviceProvider.GetRequiredService<ApplicationDbContext>();

        try
        {
            logger.LogInformation("Checking for pending database migrations...");

            // Check if database exists and create it if it doesn't
            await context.Database.EnsureCreatedAsync();

            // Get pending migrations
            var pendingMigrations = await context.Database.GetPendingMigrationsAsync();

            if (pendingMigrations.Any())
            {
                logger.LogInformation("Found {Count} pending migrations. Applying...", pendingMigrations.Count());
                await context.Database.MigrateAsync();
                logger.LogInformation("Database migrations applied successfully.");
            }
            else
            {
                logger.LogInformation("Database is up to date. No pending migrations found.");
            }
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "An error occurred while applying database migrations. Make sure PostgreSQL is running and connection string is correct.");

            // In production, you might want to throw the exception to prevent the app from starting
            // throw;
        }
    }

    /// <summary>
    /// Seeds initial data into the database (if needed)
    /// </summary>
    /// <param name="serviceProvider">The service provider</param>
    /// <returns>A task representing the asynchronous operation</returns>
    public static async Task SeedDatabaseAsync(IServiceProvider serviceProvider)
    {
        var logger = serviceProvider.GetRequiredService<ILogger<Program>>();
        
        try
        {
            logger.LogInformation("Checking for database seeding requirements...");
            
            // Add your seeding logic here if needed
            // For example:
            // var context = serviceProvider.GetRequiredService<ApplicationDbContext>();
            // if (!await context.TempTestRecords.AnyAsync())
            // {
            //     // Add initial data
            // }
            
            logger.LogInformation("Database seeding completed.");
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "An error occurred while seeding the database.");
        }
    }
}