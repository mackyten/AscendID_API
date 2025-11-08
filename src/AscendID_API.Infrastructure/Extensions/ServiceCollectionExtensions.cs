using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using AscendID_API.Application.Interfaces;
using AscendID_API.Infrastructure.Data;
using AscendID_API.Infrastructure.Repositories;

namespace AscendID_API.Infrastructure.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        // Add DbContext
        services.AddDbContext<ApplicationDbContext>(options =>
            options.UseSqlite(configuration.GetConnectionString("DefaultConnection")));

        // Add Repositories
        services.AddScoped<ITempTestRecordRepository, TempTestRecordRepository>();

        return services;
    }
}