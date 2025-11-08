using AscendID_API.Domain.Entities.Identity;

namespace AscendID_API.Application.Interfaces.Identity;

/// <summary>
/// Service interface for OAuth client management
/// </summary>
public interface IOAuthClientService
{
    /// <summary>
    /// Gets an OAuth client by its ID
    /// </summary>
    /// <param name="clientId">The client ID</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The client if found, null otherwise</returns>
    Task<OAuthClient?> GetClientByIdAsync(string clientId, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Gets all OAuth clients
    /// </summary>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>List of OAuth clients</returns>
    Task<IEnumerable<OAuthClient>> GetAllClientsAsync(CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Creates a new OAuth client
    /// </summary>
    /// <param name="client">The client to create</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>True if successful, false otherwise</returns>
    Task<bool> CreateClientAsync(OAuthClient client, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Updates an existing OAuth client
    /// </summary>
    /// <param name="client">The client to update</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>True if successful, false otherwise</returns>
    Task<bool> UpdateClientAsync(OAuthClient client, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Deletes an OAuth client
    /// </summary>
    /// <param name="clientId">The client ID</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>True if successful, false otherwise</returns>
    Task<bool> DeleteClientAsync(string clientId, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Validates an OAuth client credentials
    /// </summary>
    /// <param name="clientId">The client ID</param>
    /// <param name="clientSecret">The client secret</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>True if credentials are valid, false otherwise</returns>
    Task<bool> ValidateClientCredentialsAsync(string clientId, string clientSecret, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Validates if a redirect URI is allowed for the client
    /// </summary>
    /// <param name="clientId">The client ID</param>
    /// <param name="redirectUri">The redirect URI</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>True if redirect URI is allowed, false otherwise</returns>
    Task<bool> ValidateRedirectUriAsync(string clientId, string redirectUri, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Validates if scopes are allowed for the client
    /// </summary>
    /// <param name="clientId">The client ID</param>
    /// <param name="requestedScopes">The requested scopes</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>True if all scopes are allowed, false otherwise</returns>
    Task<bool> ValidateScopesAsync(string clientId, IEnumerable<string> requestedScopes, CancellationToken cancellationToken = default);
}