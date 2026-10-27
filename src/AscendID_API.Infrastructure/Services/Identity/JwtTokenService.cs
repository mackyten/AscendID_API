using System.Security.Claims;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Text;
using AscendID_API.Application.Interfaces.Identity;
using AscendID_API.Domain.Entities.Identity;

namespace AscendID_API.Infrastructure.Services.Identity;

/// <summary>
/// JWT token service implementation using System.IdentityModel.Tokens.Jwt
/// </summary>
public class JwtTokenService : IJwtTokenService
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IConfiguration _configuration;
    private readonly ILogger<JwtTokenService> _logger;
    private readonly JwtSecurityTokenHandler _tokenHandler;

    // In-memory token storage for simplicity (use Redis or database in production)
    private readonly Dictionary<string, TokenInfo> _tokenStorage = new();

    public JwtTokenService(
        UserManager<ApplicationUser> userManager,
        IConfiguration configuration,
        ILogger<JwtTokenService> logger)
    {
        _userManager = userManager;
        _configuration = configuration;
        _logger = logger;
        _tokenHandler = new JwtSecurityTokenHandler();
    }

    public Task<TokenResult> GenerateTokensAsync(
        Guid userId,
        string userName,
        string email,
        IEnumerable<string> roles,
        IEnumerable<string> scopes,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var jwtSettings = GetJwtSettings();
            var key = Encoding.ASCII.GetBytes(jwtSettings.SecretKey);

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, userId.ToString()),
                new Claim(ClaimTypes.Name, userName),
                new Claim(ClaimTypes.Email, email),
                new Claim(JwtRegisteredClaimNames.Sub, userId.ToString()),
                new Claim(JwtRegisteredClaimNames.Email, email),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(JwtRegisteredClaimNames.Iat, DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString(), ClaimValueTypes.Integer64)
            };

            // Add role claims
            foreach (var role in roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
                claims.Add(new Claim("role", role)); // Standard claim
            }

            // Add scope claims
            foreach (var scope in scopes)
            {
                claims.Add(new Claim("scope", scope));
            }

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.Add(jwtSettings.AccessTokenLifetime),
                Issuer = jwtSettings.Issuer,
                Audience = jwtSettings.Audience,
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            var token = _tokenHandler.CreateToken(tokenDescriptor);
            var accessToken = _tokenHandler.WriteToken(token);

            // Generate refresh token
            var refreshToken = GenerateRefreshToken();

            // Store token info
            var tokenInfo = new TokenInfo
            {
                UserId = userId,
                AccessToken = accessToken,
                RefreshToken = refreshToken,
                ExpiresAt = tokenDescriptor.Expires.Value,
                CreatedAt = DateTime.UtcNow,
                IsRevoked = false
            };

            _tokenStorage[accessToken] = tokenInfo;
            _tokenStorage[refreshToken] = tokenInfo;

            return Task.FromResult(new TokenResult
            {
                AccessToken = accessToken,
                RefreshToken = refreshToken,
                ExpiresAt = tokenDescriptor.Expires.Value,
                TokenType = "Bearer"
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to generate tokens for user {UserId}", userId);
            throw;
        }
    }

    public Task<Application.Interfaces.Identity.TokenValidationResult> ValidateAccessTokenAsync(
        string accessToken,
        CancellationToken cancellationToken = default)
    {
        try
        {
            if (string.IsNullOrEmpty(accessToken))
            {
                return Task.FromResult(new Application.Interfaces.Identity.TokenValidationResult 
                { 
                    IsValid = false, 
                    ErrorMessage = "Token is null or empty" 
                });
            }

            // Check if token is in storage and not revoked
            if (_tokenStorage.TryGetValue(accessToken, out var tokenInfo))
            {
                if (tokenInfo.IsRevoked)
                {
                    return Task.FromResult(new Application.Interfaces.Identity.TokenValidationResult 
                    { 
                        IsValid = false, 
                        ErrorMessage = "Token has been revoked" 
                    });
                }

                if (tokenInfo.ExpiresAt < DateTime.UtcNow)
                {
                    return Task.FromResult(new Application.Interfaces.Identity.TokenValidationResult 
                    { 
                        IsValid = false, 
                        ErrorMessage = "Token has expired" 
                    });
                }
            }

            var jwtSettings = GetJwtSettings();
            var key = Encoding.ASCII.GetBytes(jwtSettings.SecretKey);

            var tokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(key),
                ValidateIssuer = true,
                ValidIssuer = jwtSettings.Issuer,
                ValidateAudience = true,
                ValidAudience = jwtSettings.Audience,
                ValidateLifetime = true,
                ClockSkew = TimeSpan.Zero
            };

            var principal = _tokenHandler.ValidateToken(accessToken, tokenValidationParameters, out var validatedToken);

            var userIdClaim = principal.FindFirst(ClaimTypes.NameIdentifier) ?? principal.FindFirst(JwtRegisteredClaimNames.Sub);
            var roles = principal.FindAll(ClaimTypes.Role).Select(c => c.Value);
            var scopes = principal.FindAll("scope").Select(c => c.Value);

            return Task.FromResult(new Application.Interfaces.Identity.TokenValidationResult
            {
                IsValid = true,
                UserId = userIdClaim != null && Guid.TryParse(userIdClaim.Value, out var userId) ? userId : null,
                Roles = roles,
                Scopes = scopes,
                ExpiresAt = validatedToken.ValidTo
            });
        }
        catch (SecurityTokenExpiredException)
        {
            return Task.FromResult(new Application.Interfaces.Identity.TokenValidationResult 
            { 
                IsValid = false, 
                ErrorMessage = "Token has expired" 
            });
        }
        catch (SecurityTokenException ex)
        {
            _logger.LogWarning(ex, "Token validation failed");
            return Task.FromResult(new Application.Interfaces.Identity.TokenValidationResult 
            { 
                IsValid = false, 
                ErrorMessage = "Invalid token" 
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to validate access token");
            return Task.FromResult(new Application.Interfaces.Identity.TokenValidationResult 
            { 
                IsValid = false, 
                ErrorMessage = "Token validation failed" 
            });
        }
    }

    public async Task<TokenResult?> RefreshTokensAsync(
        string refreshToken,
        CancellationToken cancellationToken = default)
    {
        try
        {
            if (string.IsNullOrEmpty(refreshToken))
            {
                return null;
            }

            // Find the token info
            if (!_tokenStorage.TryGetValue(refreshToken, out var tokenInfo))
            {
                return null;
            }

            if (tokenInfo.IsRevoked || tokenInfo.ExpiresAt < DateTime.UtcNow.AddDays(-30))
            {
                return null;
            }

            var user = await _userManager.FindByIdAsync(tokenInfo.UserId.ToString());
            if (user == null || !user.IsActive)
            {
                return null;
            }

            // Get user roles
            var roles = await _userManager.GetRolesAsync(user);
            var scopes = new[] { "openid", "profile", "email" }; // Default scopes

            // Revoke old tokens
            tokenInfo.IsRevoked = true;

            // Generate new tokens
            return await GenerateTokensAsync(
                user.Id,
                user.UserName ?? user.Email ?? "Unknown",
                user.Email ?? string.Empty,
                roles,
                scopes,
                cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to refresh tokens");
            return null;
        }
    }

    public Task<bool> RevokeTokenAsync(
        string token,
        CancellationToken cancellationToken = default)
    {
        try
        {
            if (string.IsNullOrEmpty(token))
            {
                return Task.FromResult(false);
            }

            if (_tokenStorage.TryGetValue(token, out var tokenInfo))
            {
                tokenInfo.IsRevoked = true;
                return Task.FromResult(true);
            }

            return Task.FromResult(false);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to revoke token");
            return Task.FromResult(false);
        }
    }

    public ClaimsPrincipal? GetPrincipalFromToken(string accessToken)
    {
        try
        {
            if (string.IsNullOrEmpty(accessToken))
            {
                return null;
            }

            var jwtSettings = GetJwtSettings();
            var key = Encoding.ASCII.GetBytes(jwtSettings.SecretKey);

            var tokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(key),
                ValidateIssuer = false,
                ValidateAudience = false,
                ValidateLifetime = false,
                ClockSkew = TimeSpan.Zero
            };

            var principal = _tokenHandler.ValidateToken(accessToken, tokenValidationParameters, out var validatedToken);
            return principal;
        }
        catch
        {
            return null;
        }
    }

    private JwtSettings GetJwtSettings()
    {
        return new JwtSettings
        {
            // "Jwt:Key" is what the JWT bearer validation reads; "Jwt:SecretKey" is the legacy name.
            SecretKey = _configuration["Jwt:Key"] ?? _configuration["Jwt:SecretKey"]
                ?? throw new InvalidOperationException("JWT signing key is not configured (Jwt:Key)."),
            Issuer = _configuration["Jwt:Issuer"] ?? "AscendID_API",
            Audience = _configuration["Jwt:Audience"] ?? "AscendID_API_Clients",
            AccessTokenLifetime = TimeSpan.FromHours(1),
            RefreshTokenLifetime = TimeSpan.FromDays(30)
        };
    }

    private string GenerateRefreshToken()
    {
        var randomBytes = new byte[64];
        using var rng = System.Security.Cryptography.RandomNumberGenerator.Create();
        rng.GetBytes(randomBytes);
        return Convert.ToBase64String(randomBytes);
    }
}

internal class JwtSettings
{
    public string SecretKey { get; set; } = string.Empty;
    public string Issuer { get; set; } = string.Empty;
    public string Audience { get; set; } = string.Empty;
    public TimeSpan AccessTokenLifetime { get; set; }
    public TimeSpan RefreshTokenLifetime { get; set; }
}

internal class TokenInfo
{
    public Guid UserId { get; set; }
    public string AccessToken { get; set; } = string.Empty;
    public string RefreshToken { get; set; } = string.Empty;
    public DateTime ExpiresAt { get; set; }
    public DateTime CreatedAt { get; set; }
    public bool IsRevoked { get; set; }
}