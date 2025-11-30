using AscendID_API.Application.Handlers;
using AscendID_API.Infrastructure.Extensions;

namespace AscendID_API.WebAPI.Configuration;

public static class ServiceConfiguration
{
    /// <summary>
    /// Configures all application services including controllers, MediatR, infrastructure, and API documentation
    /// </summary>
    /// <param name="services">The service collection</param>
    /// <param name="configuration">Application configuration</param>
    /// <returns>The configured service collection</returns>
    public static IServiceCollection ConfigureApplicationServices(this IServiceCollection services, IConfiguration configuration)
    {
        // Add configuration options
        services.Configure<AdminUserOptions>(configuration.GetSection(AdminUserOptions.SectionName));
        
        // Add controllers with views support for MVC
        services.AddControllersWithViews();

        // Add MediatR for CQRS pattern
        services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(CreateTempTestRecordHandler).Assembly));

        // Add Infrastructure services (Entity Framework, repositories, etc.)
        services.AddInfrastructure(configuration);

        // Add API documentation services
        services.ConfigureApiDocumentation();

        return services;
    }

    /// <summary>
    /// Configures OpenAPI/Swagger documentation services
    /// </summary>
    /// <param name="services">The service collection</param>
    /// <returns>The configured service collection</returns>
    private static IServiceCollection ConfigureApiDocumentation(this IServiceCollection services)
    {
        // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen(options =>
        {
            options.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
            {
                Title = "AscendID API",
                Version = "v1",
                Description = "A clean architecture API for AscendID application"
            });

            // Only include API controllers, exclude MVC controllers
            options.DocInclusionPredicate((name, apiDesc) =>
            {
                // Exclude AuthController (MVC controller)
                if (apiDesc.ActionDescriptor.RouteValues.ContainsKey("controller") &&
                    apiDesc.ActionDescriptor.RouteValues["controller"] == "Auth")
                {
                    return false;
                }
                
                // Include only API controllers (those with [ApiController] attribute or in /api/ routes)
                return apiDesc.RelativePath?.StartsWith("api/") == true;
            });
        });

        return services;
    }
}