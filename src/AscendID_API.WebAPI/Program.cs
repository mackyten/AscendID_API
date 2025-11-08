using AscendID_API.Infrastructure.Extensions;
using AscendID_API.Application.Handlers;
using Microsoft.EntityFrameworkCore;
using AscendID_API.Infrastructure.Data;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();

// Add MediatR
builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(CreateTempTestRecordHandler).Assembly));

// Add Infrastructure services
builder.Services.AddInfrastructure(builder.Configuration);

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

// Apply database migrations automatically on startup
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var logger = services.GetRequiredService<ILogger<Program>>();
    var context = services.GetRequiredService<ApplicationDbContext>();
    
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

app.Run();
