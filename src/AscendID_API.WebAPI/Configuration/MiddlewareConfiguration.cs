namespace AscendID_API.WebAPI.Configuration;

public static class MiddlewareConfiguration
{
    /// <summary>
    /// Configures the HTTP request pipeline middleware
    /// </summary>
    /// <param name="app">The web application</param>
    /// <returns>The configured web application</returns>
    public static WebApplication ConfigureMiddleware(this WebApplication app)
    {
        // Configure development-specific middleware
        app.ConfigureDevelopmentMiddleware();

        // Configure security middleware
        app.ConfigureSecurityMiddleware();

        // Configure routing and controllers
        app.ConfigureRouting();

        return app;
    }

    /// <summary>
    /// Configures middleware that should only run in development environments
    /// </summary>
    /// <param name="app">The web application</param>
    /// <returns>The configured web application</returns>
    private static WebApplication ConfigureDevelopmentMiddleware(this WebApplication app)
    {
        // Enable Swagger in development and local environments
        if (app.Environment.IsDevelopment() || app.Environment.EnvironmentName == "Local")
        {
            app.UseSwagger();
            app.UseSwaggerUI(options =>
            {
                options.SwaggerEndpoint("/swagger/v1/swagger.json", "AscendID API v1");
                options.RoutePrefix = "swagger";
                options.DisplayRequestDuration();
            });
        }

        return app;
    }

    /// <summary>
    /// Configures security-related middleware
    /// </summary>
    /// <param name="app">The web application</param>
    /// <returns>The configured web application</returns>
    private static WebApplication ConfigureSecurityMiddleware(this WebApplication app)
    {
        // Force HTTPS redirection
        app.UseHttpsRedirection();

        // Add authentication and authorization
        app.UseAuthorization();

        return app;
    }

    /// <summary>
    /// Configures routing and controller mapping
    /// </summary>
    /// <param name="app">The web application</param>
    /// <returns>The configured web application</returns>
    private static WebApplication ConfigureRouting(this WebApplication app)
    {
        // Map controller routes
        app.MapControllers();

        return app;
    }
}