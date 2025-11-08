namespace AscendID_API.Application.Interfaces.Identity;

/// <summary>
/// Service interface for authentication operations
/// </summary>
public interface IAuthenticationService
{
    /// <summary>
    /// Signs in a user with email and password
    /// </summary>
    /// <param name="email">The user's email</param>
    /// <param name="password">The user's password</param>
    /// <param name="rememberMe">Whether to remember the user</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Sign-in result</returns>
    Task<AuthenticationResult> SignInAsync(string email, string password, bool rememberMe = false, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Signs out the current user
    /// </summary>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Task</returns>
    Task SignOutAsync(CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Refreshes an access token using a refresh token
    /// </summary>
    /// <param name="refreshToken">The refresh token</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>New token pair if successful</returns>
    Task<TokenResult?> RefreshTokenAsync(string refreshToken, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Revokes a refresh token
    /// </summary>
    /// <param name="refreshToken">The refresh token to revoke</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>True if successful</returns>
    Task<bool> RevokeTokenAsync(string refreshToken, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Validates an access token
    /// </summary>
    /// <param name="accessToken">The access token to validate</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Validation result</returns>
    Task<TokenValidationResult> ValidateTokenAsync(string accessToken, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Gets the current authenticated user ID from the context
    /// </summary>
    /// <returns>User ID if authenticated, null otherwise</returns>
    Guid? GetCurrentUserId();
    
    /// <summary>
    /// Checks if the current user is authenticated
    /// </summary>
    /// <returns>True if authenticated, false otherwise</returns>
    bool IsAuthenticated();
}

/// <summary>
/// Result of an authentication attempt
/// </summary>
public class AuthenticationResult
{
    public bool Success { get; set; }
    public string? AccessToken { get; set; }
    public string? RefreshToken { get; set; }
    public DateTime? ExpiresAt { get; set; }
    public string? ErrorMessage { get; set; }
    public bool RequiresTwoFactor { get; set; }
    public bool IsLockedOut { get; set; }
    public bool IsNotAllowed { get; set; }
}

/// <summary>
/// Result of a token operation
/// </summary>
public class TokenResult
{
    public string AccessToken { get; set; } = string.Empty;
    public string RefreshToken { get; set; } = string.Empty;
    public DateTime ExpiresAt { get; set; }
    public string TokenType { get; set; } = "Bearer";
}

/// <summary>
/// Result of token validation
/// </summary>
public class TokenValidationResult
{
    public bool IsValid { get; set; }
    public Guid? UserId { get; set; }
    public IEnumerable<string> Roles { get; set; } = Enumerable.Empty<string>();
    public IEnumerable<string> Scopes { get; set; } = Enumerable.Empty<string>();
    public DateTime? ExpiresAt { get; set; }
    public string? ErrorMessage { get; set; }
}