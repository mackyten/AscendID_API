using System.Security.Claims;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Http;
using AscendID_API.Application.Interfaces.Identity;
using AscendID_API.Domain.Entities.Identity;

namespace AscendID_API.Infrastructure.Services.Identity;

/// <summary>
/// Implementation of authentication service
/// </summary>
public class AuthenticationService : IAuthenticationService
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly SignInManager<ApplicationUser> _signInManager;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IUserService _userService;

    public AuthenticationService(
        UserManager<ApplicationUser> userManager,
        SignInManager<ApplicationUser> signInManager,
        IHttpContextAccessor httpContextAccessor,
        IUserService userService)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _httpContextAccessor = httpContextAccessor;
        _userService = userService;
    }

    public async Task<AuthenticationResult> SignInAsync(string email, string password, bool rememberMe = false, CancellationToken cancellationToken = default)
    {
        var user = await _userManager.FindByEmailAsync(email);
        if (user == null || !user.IsActive)
        {
            return new AuthenticationResult
            {
                Success = false,
                ErrorMessage = "Invalid email or password"
            };
        }

        var result = await _signInManager.PasswordSignInAsync(user, password, rememberMe, lockoutOnFailure: true);

        if (result.Succeeded)
        {
            // Update last login timestamp
            await _userService.UpdateLastLoginAsync(user.Id, cancellationToken);

            return new AuthenticationResult
            {
                Success = true,
                // Note: In a real OIDC implementation, you would generate actual JWT tokens here
                // For now, this relies on cookie authentication
                AccessToken = "cookie-based", // Placeholder
                ExpiresAt = DateTime.UtcNow.AddHours(1) // Placeholder
            };
        }

        var authResult = new AuthenticationResult { Success = false };

        if (result.IsLockedOut)
        {
            authResult.IsLockedOut = true;
            authResult.ErrorMessage = "Account is locked due to multiple failed login attempts";
        }
        else if (result.RequiresTwoFactor)
        {
            authResult.RequiresTwoFactor = true;
            authResult.ErrorMessage = "Two-factor authentication is required";
        }
        else if (result.IsNotAllowed)
        {
            authResult.IsNotAllowed = true;
            authResult.ErrorMessage = "User is not allowed to sign in";
        }
        else
        {
            authResult.ErrorMessage = "Invalid email or password";
        }

        return authResult;
    }

    public async Task SignOutAsync(CancellationToken cancellationToken = default)
    {
        await _signInManager.SignOutAsync();
    }

    public async Task<TokenResult?> RefreshTokenAsync(string refreshToken, CancellationToken cancellationToken = default)
    {
        // TODO: Implement refresh token logic with OpenIddict
        // This is a placeholder implementation
        await Task.CompletedTask;
        return null;
    }

    public async Task<bool> RevokeTokenAsync(string refreshToken, CancellationToken cancellationToken = default)
    {
        // TODO: Implement token revocation logic with OpenIddict
        // This is a placeholder implementation
        await Task.CompletedTask;
        return false;
    }

    public async Task<TokenValidationResult> ValidateTokenAsync(string accessToken, CancellationToken cancellationToken = default)
    {
        // TODO: Implement token validation logic with OpenIddict
        // This is a placeholder implementation
        await Task.CompletedTask;
        return new TokenValidationResult { IsValid = false };
    }

    public Guid? GetCurrentUserId()
    {
        var userIdClaim = _httpContextAccessor.HttpContext?.User.FindFirst(ClaimTypes.NameIdentifier);
        if (userIdClaim != null && Guid.TryParse(userIdClaim.Value, out var userId))
        {
            return userId;
        }

        return null;
    }

    public bool IsAuthenticated()
    {
        return _httpContextAccessor.HttpContext?.User.Identity?.IsAuthenticated == true;
    }
}