using AscendID_API.Infrastructure.Data;
using AscendID_API.Domain.Entities.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;

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
        await SeedDatabaseAsync(scope.ServiceProvider);
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
            
            var userManager = serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();
            var roleManager = serviceProvider.GetRequiredService<RoleManager<ApplicationRole>>();

            await SeedRolesAsync(roleManager, logger);
            await SeedAdminUserAsync(userManager, roleManager, serviceProvider, logger);
            
            logger.LogInformation("Database seeding completed.");
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "An error occurred while seeding the database.");
        }
    }

    /// <summary>
    /// Seeds initial roles into the database
    /// </summary>
    /// <param name="roleManager">The role manager</param>
    /// <param name="logger">The logger</param>
    /// <returns>A task representing the asynchronous operation</returns>
    private static async Task SeedRolesAsync(RoleManager<ApplicationRole> roleManager, ILogger logger)
    {
        var roles = new[]
        {
            new { Name = "Administrator", Description = "System administrator with full access", IsSystemRole = true },
            new { Name = "User", Description = "Standard user with basic access", IsSystemRole = true },
            new { Name = "Manager", Description = "Manager with elevated privileges", IsSystemRole = false }
        };

        foreach (var roleData in roles)
        {
            if (!await roleManager.RoleExistsAsync(roleData.Name))
            {
                var role = new ApplicationRole
                {
                    Name = roleData.Name,
                    Description = roleData.Description,
                    IsSystemRole = roleData.IsSystemRole,
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow
                };

                var result = await roleManager.CreateAsync(role);
                if (result.Succeeded)
                {
                    logger.LogInformation("Created role: {RoleName}", roleData.Name);
                }
                else
                {
                    logger.LogWarning("Failed to create role {RoleName}: {Errors}", roleData.Name, 
                        string.Join(", ", result.Errors.Select(e => e.Description)));
                }
            }
        }
    }

    /// <summary>
    /// Seeds the initial administrator user
    /// </summary>
    /// <param name="userManager">The user manager</param>
    /// <param name="roleManager">The role manager</param>
    /// <param name="serviceProvider">The service provider</param>
    /// <param name="logger">The logger</param>
    /// <returns>A task representing the asynchronous operation</returns>
    private static async Task SeedAdminUserAsync(UserManager<ApplicationUser> userManager, RoleManager<ApplicationRole> roleManager, IServiceProvider serviceProvider, ILogger logger)
    {
        // Get admin user configuration from appsettings
        var configuration = serviceProvider.GetRequiredService<IConfiguration>();
        var adminOptions = configuration.GetSection(AdminUserOptions.SectionName).Get<AdminUserOptions>();
        
        if (adminOptions == null || string.IsNullOrEmpty(adminOptions.Email) || string.IsNullOrEmpty(adminOptions.Password))
        {
            logger.LogWarning("Admin user configuration is missing or incomplete. Skipping admin user creation.");
            return;
        }
        
        var adminUser = await userManager.FindByEmailAsync(adminOptions.Email);
        
        if (adminUser == null)
        {
            adminUser = new ApplicationUser
            {
                UserName = adminOptions.Email,
                Email = adminOptions.Email,
                EmailConfirmed = true,
                FirstName = "System",
                LastName = "Administrator",
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            };

            var result = await userManager.CreateAsync(adminUser, adminOptions.Password);
            
            if (result.Succeeded)
            {
                logger.LogInformation("Created admin user: {Email}", adminOptions.Email);
                
                // Add to Administrator role
                if (await roleManager.RoleExistsAsync("Administrator"))
                {
                    await userManager.AddToRoleAsync(adminUser, "Administrator");
                    logger.LogInformation("Added admin user to Administrator role");
                }
            }
            else
            {
                logger.LogWarning("Failed to create admin user: {Errors}", 
                    string.Join(", ", result.Errors.Select(e => e.Description)));
            }
        }
        else
        {
            logger.LogInformation("Admin user already exists");
        }
    }
}