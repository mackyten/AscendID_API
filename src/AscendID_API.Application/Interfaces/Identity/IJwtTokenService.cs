using System.Security.Claims;

namespace AscendID_API.Application.Interfaces.Identity;

/// <summary>
/// Service interface for JWT token operations
/// </summary>
public interface IJwtTokenService
{
    /// <summary>
    /// Generates access and refresh tokens for a user
    /// </summary>
    /// <param name="userId">The user ID</param>
    /// <param name="userName">The username</param>
    /// <param name="email">The user's email</param>
    /// <param name="roles">User roles</param>
    /// <param name="scopes">Token scopes</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Token result with access and refresh tokens</returns>
    Task<TokenResult> GenerateTokensAsync(
        Guid userId,
        string userName,
        string email,
        IEnumerable<string> roles,
        IEnumerable<string> scopes,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Validates an access token
    /// </summary>
    /// <param name="accessToken">The access token to validate</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Token validation result</returns>
    Task<TokenValidationResult> ValidateAccessTokenAsync(
        string accessToken,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Refreshes tokens using a refresh token
    /// </summary>
    /// <param name="refreshToken">The refresh token</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>New token pair if successful</returns>
    Task<TokenResult?> RefreshTokensAsync(
        string refreshToken,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Revokes a token
    /// </summary>
    /// <param name="token">The token to revoke</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>True if successful</returns>
    Task<bool> RevokeTokenAsync(
        string token,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Extracts claims from an access token
    /// </summary>
    /// <param name="accessToken">The access token</param>
    /// <returns>Claims principal if valid, null otherwise</returns>
    ClaimsPrincipal? GetPrincipalFromToken(string accessToken);
}