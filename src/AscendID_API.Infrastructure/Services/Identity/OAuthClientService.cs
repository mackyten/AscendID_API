using Microsoft.EntityFrameworkCore;
using AscendID_API.Application.Interfaces.Identity;
using AscendID_API.Domain.Entities.Identity;
using AscendID_API.Infrastructure.Data;

namespace AscendID_API.Infrastructure.Services.Identity;

/// <summary>
/// Implementation of OAuth client service
/// </summary>
public class OAuthClientService : IOAuthClientService
{
    private readonly ApplicationDbContext _context;

    public OAuthClientService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<OAuthClient?> GetClientByIdAsync(string clientId, CancellationToken cancellationToken = default)
    {
        return await _context.OAuthClients
            .FirstOrDefaultAsync(c => c.ClientId == clientId && c.IsEnabled, cancellationToken);
    }

    public async Task<IEnumerable<OAuthClient>> GetAllClientsAsync(CancellationToken cancellationToken = default)
    {
        return await _context.OAuthClients
            .Where(c => c.IsEnabled)
            .OrderBy(c => c.ClientName)
            .ToListAsync(cancellationToken);
    }

    public async Task<bool> CreateClientAsync(OAuthClient client, CancellationToken cancellationToken = default)
    {
        client.Id = Guid.NewGuid();
        client.CreatedAt = DateTime.UtcNow;

        _context.OAuthClients.Add(client);
        var result = await _context.SaveChangesAsync(cancellationToken);
        
        return result > 0;
    }

    public async Task<bool> UpdateClientAsync(OAuthClient client, CancellationToken cancellationToken = default)
    {
        client.UpdatedAt = DateTime.UtcNow;

        _context.OAuthClients.Update(client);
        var result = await _context.SaveChangesAsync(cancellationToken);
        
        return result > 0;
    }

    public async Task<bool> DeleteClientAsync(string clientId, CancellationToken cancellationToken = default)
    {
        var client = await GetClientByIdAsync(clientId, cancellationToken);
        if (client == null)
            return false;

        _context.OAuthClients.Remove(client);
        var result = await _context.SaveChangesAsync(cancellationToken);
        
        return result > 0;
    }

    public async Task<bool> ValidateClientCredentialsAsync(string clientId, string clientSecret, CancellationToken cancellationToken = default)
    {
        var client = await GetClientByIdAsync(clientId, cancellationToken);
        if (client == null)
            return false;

        // TODO: Implement proper password hashing for client secrets
        // For now, using plain text comparison (NOT SECURE for production)
        return client.ClientSecret == clientSecret;
    }

    public async Task<bool> ValidateRedirectUriAsync(string clientId, string redirectUri, CancellationToken cancellationToken = default)
    {
        var client = await GetClientByIdAsync(clientId, cancellationToken);
        if (client == null)
            return false;

        var allowedUris = client.RedirectUris.Split('\n', StringSplitOptions.RemoveEmptyEntries);
        return allowedUris.Any(uri => uri.Trim().Equals(redirectUri, StringComparison.OrdinalIgnoreCase));
    }

    public async Task<bool> ValidateScopesAsync(string clientId, IEnumerable<string> requestedScopes, CancellationToken cancellationToken = default)
    {
        var client = await GetClientByIdAsync(clientId, cancellationToken);
        if (client == null)
            return false;

        var allowedScopes = client.AllowedScopes.Split(' ', StringSplitOptions.RemoveEmptyEntries);
        return requestedScopes.All(scope => allowedScopes.Contains(scope, StringComparer.OrdinalIgnoreCase));
    }
}