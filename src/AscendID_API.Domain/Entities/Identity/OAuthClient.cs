namespace AscendID_API.Domain.Entities.Identity;

/// <summary>
/// Represents an OAuth client application that can request tokens
/// </summary>
public class OAuthClient
{
    /// <summary>
    /// Gets or sets the unique identifier for the client
    /// </summary>
    public Guid Id { get; set; }
    
    /// <summary>
    /// Gets or sets the client identifier used in OAuth flows
    /// </summary>
    public string ClientId { get; set; } = string.Empty;
    
    /// <summary>
    /// Gets or sets the display name of the client application
    /// </summary>
    public string ClientName { get; set; } = string.Empty;
    
    /// <summary>
    /// Gets or sets the client secret (hashed)
    /// </summary>
    public string? ClientSecret { get; set; }
    
    /// <summary>
    /// Gets or sets the description of the client application
    /// </summary>
    public string? Description { get; set; }
    
    /// <summary>
    /// Gets or sets the allowed grant types (space-separated)
    /// </summary>
    public string AllowedGrantTypes { get; set; } = string.Empty;
    
    /// <summary>
    /// Gets or sets the allowed scopes (space-separated)
    /// </summary>
    public string AllowedScopes { get; set; } = string.Empty;
    
    /// <summary>
    /// Gets or sets the redirect URIs (newline-separated)
    /// </summary>
    public string RedirectUris { get; set; } = string.Empty;
    
    /// <summary>
    /// Gets or sets the post logout redirect URIs (newline-separated)
    /// </summary>
    public string? PostLogoutRedirectUris { get; set; }
    
    /// <summary>
    /// Gets or sets the allowed CORS origins (newline-separated)
    /// </summary>
    public string? AllowedCorsOrigins { get; set; }
    
    /// <summary>
    /// Gets or sets a value indicating whether the client is enabled
    /// </summary>
    public bool IsEnabled { get; set; } = true;
    
    /// <summary>
    /// Gets or sets a value indicating whether consent is required
    /// </summary>
    public bool RequireConsent { get; set; } = false;
    
    /// <summary>
    /// Gets or sets a value indicating whether PKCE is required
    /// </summary>
    public bool RequirePkce { get; set; } = true;
    
    /// <summary>
    /// Gets or sets the access token lifetime in seconds
    /// </summary>
    public int AccessTokenLifetime { get; set; } = 3600; // 1 hour
    
    /// <summary>
    /// Gets or sets the refresh token lifetime in seconds
    /// </summary>
    public int RefreshTokenLifetime { get; set; } = 2592000; // 30 days
    
    /// <summary>
    /// Gets or sets the date when the client was created
    /// </summary>
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    
    /// <summary>
    /// Gets or sets the date when the client was last updated
    /// </summary>
    public DateTime? UpdatedAt { get; set; }
}