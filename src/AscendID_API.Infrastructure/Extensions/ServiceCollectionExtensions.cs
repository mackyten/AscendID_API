using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Identity;
using AscendID_API.Application.Interfaces;
using AscendID_API.Application.Interfaces.Identity;
using AscendID_API.Domain.Entities.Identity;
using AscendID_API.Infrastructure.Data;
using AscendID_API.Infrastructure.Repositories;
using AscendID_API.Infrastructure.Services.Identity;

namespace AscendID_API.Infrastructure.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        // Add DbContext
        services.AddDbContext<ApplicationDbContext>(options =>
            options.UseNpgsql(configuration.GetConnectionString("DefaultConnection"))
                   .UseOpenIddict());

        // Add Identity
        services.AddIdentity<ApplicationUser, ApplicationRole>(options =>
        {
            // Password settings
            options.Password.RequiredLength = 8;
            options.Password.RequireDigit = true;
            options.Password.RequireLowercase = true;
            options.Password.RequireUppercase = true;
            options.Password.RequireNonAlphanumeric = true;
            options.Password.RequiredUniqueChars = 1;

            // Lockout settings
            options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
            options.Lockout.MaxFailedAccessAttempts = 5;
            options.Lockout.AllowedForNewUsers = true;

            // User settings
            options.User.AllowedUserNameCharacters =
                "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-._@+";
            options.User.RequireUniqueEmail = true;

            // Sign-in settings
            options.SignIn.RequireConfirmedEmail = false;
            options.SignIn.RequireConfirmedPhoneNumber = false;
        })
        .AddEntityFrameworkStores<ApplicationDbContext>()
        .AddDefaultTokenProviders();

        // Add OpenIddict
        services.AddOpenIddict()
            .AddCore(options =>
            {
                options.UseEntityFrameworkCore()
                       .UseDbContext<ApplicationDbContext>();
            })
            .AddServer(options =>
            {
                // Enable the authorization, logout, token and userinfo endpoints
                options.SetAuthorizationEndpointUris("/connect/authorize")
                       .SetLogoutEndpointUris("/connect/logout")
                       .SetTokenEndpointUris("/connect/token")
                       .SetUserinfoEndpointUris("/connect/userinfo");

                // Mark the "email", "profile" and "roles" scopes as supported scopes
                options.RegisterScopes("openid", "email", "profile", "roles");

                // Note: this sample only uses the authorization code flow but you can enable
                // the other flows if you need to support implicit, password or client credentials
                options.AllowAuthorizationCodeFlow()
                       .AllowRefreshTokenFlow();

                // Register the signing and encryption credentials
                options.AddDevelopmentEncryptionCertificate()
                       .AddDevelopmentSigningCertificate();

                // Register the ASP.NET Core host and configure the ASP.NET Core-specific options
                options.UseAspNetCore()
                       .EnableAuthorizationEndpointPassthrough()
                       .EnableLogoutEndpointPassthrough()
                       .EnableTokenEndpointPassthrough()
                       .EnableUserinfoEndpointPassthrough()
                       .EnableStatusCodePagesIntegration();
            })
            .AddValidation(options =>
            {
                options.UseLocalServer();
                options.UseAspNetCore();
            });

        // Add application services
        services.AddHttpContextAccessor();

        // Add Repositories
        services.AddScoped<ITempTestRecordRepository, TempTestRecordRepository>();

        // Add Identity Services
        services.AddScoped<IUserService, UserService>();
        services.AddScoped<IRoleService, RoleService>();
        services.AddScoped<IAuthenticationService, AuthenticationService>();
        services.AddScoped<IOAuthClientService, OAuthClientService>();

        // Add OpenIddict configuration service
        services.AddHostedService<OpenIddictConfigurationService>();

        // Configure authentication
        services.ConfigureApplicationCookie(options =>
        {
            options.LoginPath = "/Account/Login";
            options.LogoutPath = "/Account/Logout";
            options.AccessDeniedPath = "/Account/AccessDenied";
            options.ExpireTimeSpan = TimeSpan.FromHours(1);
            options.SlidingExpiration = true;
            options.Cookie.HttpOnly = true;
            options.Cookie.SecurePolicy = Microsoft.AspNetCore.Http.CookieSecurePolicy.SameAsRequest;
            options.Cookie.SameSite = Microsoft.AspNetCore.Http.SameSiteMode.Lax;
        });

        return services;
    }
}