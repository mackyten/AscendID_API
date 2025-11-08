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

// Ensure database is created (for development)
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    try
    {
        // For PostgreSQL, use EnsureCreated() or Migrations in production
        context.Database.EnsureCreated();
    }
    catch (Exception ex)
    {
        // Log the exception or handle PostgreSQL connection issues
        var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();
        logger.LogWarning(ex, "Could not create database. Make sure PostgreSQL is running and connection string is correct.");
    }
}

app.Run();
