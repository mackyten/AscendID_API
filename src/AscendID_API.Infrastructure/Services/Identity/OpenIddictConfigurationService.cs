using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using OpenIddict.Abstractions;

namespace AscendID_API.Infrastructure.Services.Identity;

/// <summary>
/// Background service to configure OpenIddict applications and scopes
/// </summary>
public class OpenIddictConfigurationService : IHostedService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<OpenIddictConfigurationService> _logger;

    public OpenIddictConfigurationService(IServiceProvider serviceProvider, ILogger<OpenIddictConfigurationService> logger)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        using var scope = _serviceProvider.CreateScope();
        
        var manager = scope.ServiceProvider.GetRequiredService<IOpenIddictApplicationManager>();
        var scopeManager = scope.ServiceProvider.GetRequiredService<IOpenIddictScopeManager>();

        await CreateScopesAsync(scopeManager, cancellationToken);
        await CreateApplicationsAsync(manager, cancellationToken);
    }

    public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;

    private async Task CreateScopesAsync(IOpenIddictScopeManager manager, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Creating OpenIddict scopes...");

        var scopes = new[]
        {
            new { Name = "api", DisplayName = "API Access", Description = "Access to the AscendID API" },
            new { Name = "roles", DisplayName = "Roles", Description = "Access to user roles" }
        };

        foreach (var scope in scopes)
        {
            if (await manager.FindByNameAsync(scope.Name, cancellationToken) is null)
            {
                await manager.CreateAsync(new OpenIddictScopeDescriptor
                {
                    Name = scope.Name,
                    DisplayName = scope.DisplayName,
                    Description = scope.Description,
                    Resources = { "ascend-api" }
                }, cancellationToken);

                _logger.LogInformation("Created scope: {ScopeName}", scope.Name);
            }
        }
    }

    private async Task CreateApplicationsAsync(IOpenIddictApplicationManager manager, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Creating OpenIddict applications...");

        // Create a test client application
        if (await manager.FindByClientIdAsync("ascend-client", cancellationToken) is null)
        {
            await manager.CreateAsync(new OpenIddictApplicationDescriptor
            {
                ClientId = "ascend-client",
                ClientSecret = "ascend-client-secret-change-in-production",
                ConsentType = OpenIddictConstants.ConsentTypes.Explicit,
                DisplayName = "AscendID Test Client",
                ClientType = OpenIddictConstants.ClientTypes.Confidential,
                PostLogoutRedirectUris =
                {
                    new Uri("https://localhost:5001/signout-callback-oidc"),
                    new Uri("https://localhost:3000/signout-callback")
                },
                RedirectUris =
                {
                    new Uri("https://localhost:5001/signin-oidc"),
                    new Uri("https://localhost:3000/callback")
                },
                Permissions =
                {
                    OpenIddictConstants.Permissions.Endpoints.Authorization,
                    OpenIddictConstants.Permissions.Endpoints.Logout,
                    OpenIddictConstants.Permissions.Endpoints.Token,
                    OpenIddictConstants.Permissions.Endpoints.Revocation,

                    OpenIddictConstants.Permissions.GrantTypes.AuthorizationCode,
                    OpenIddictConstants.Permissions.GrantTypes.RefreshToken,

                    OpenIddictConstants.Permissions.ResponseTypes.Code,

                    OpenIddictConstants.Permissions.Scopes.Email,
                    OpenIddictConstants.Permissions.Scopes.Profile,
                    OpenIddictConstants.Permissions.Scopes.Roles,

                    OpenIddictConstants.Permissions.Prefixes.Scope + "api"
                }
            }, cancellationToken);

            _logger.LogInformation("Created application: ascend-client");
        }

        // Create a public client for SPA applications
        if (await manager.FindByClientIdAsync("ascend-spa", cancellationToken) is null)
        {
            await manager.CreateAsync(new OpenIddictApplicationDescriptor
            {
                ClientId = "ascend-spa",
                ConsentType = OpenIddictConstants.ConsentTypes.Explicit,
                DisplayName = "AscendID SPA Client",
                ClientType = OpenIddictConstants.ClientTypes.Public,
                PostLogoutRedirectUris =
                {
                    new Uri("https://localhost:3000/signout-callback"),
                    new Uri("http://localhost:3000/signout-callback")
                },
                RedirectUris =
                {
                    new Uri("https://localhost:3000/callback"),
                    new Uri("http://localhost:3000/callback")
                },
                Requirements =
                {
                    OpenIddictConstants.Requirements.Features.ProofKeyForCodeExchange
                },
                Permissions =
                {
                    OpenIddictConstants.Permissions.Endpoints.Authorization,
                    OpenIddictConstants.Permissions.Endpoints.Logout,
                    OpenIddictConstants.Permissions.Endpoints.Token,
                    OpenIddictConstants.Permissions.Endpoints.Revocation,

                    OpenIddictConstants.Permissions.GrantTypes.AuthorizationCode,
                    OpenIddictConstants.Permissions.GrantTypes.RefreshToken,

                    OpenIddictConstants.Permissions.ResponseTypes.Code,

                    OpenIddictConstants.Permissions.Scopes.Email,
                    OpenIddictConstants.Permissions.Scopes.Profile,
                    OpenIddictConstants.Permissions.Scopes.Roles,

                    OpenIddictConstants.Permissions.Prefixes.Scope + "api"
                }
            }, cancellationToken);

            _logger.LogInformation("Created application: ascend-spa");
        }
    }
}