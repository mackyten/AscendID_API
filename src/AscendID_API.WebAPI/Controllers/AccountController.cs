using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using AscendID_API.Application.Interfaces.Identity;
using AscendID_API.Domain.Entities.Identity;

namespace AscendID_API.WebAPI.Controllers;

/// <summary>
/// Controller for account management operations
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class AccountController : ControllerBase
{
    private readonly IUserService _userService;
    private readonly IAuthenticationService _authenticationService;

    public AccountController(IUserService userService, IAuthenticationService authenticationService)
    {
        _userService = userService;
        _authenticationService = authenticationService;
    }

    /// <summary>
    /// User login endpoint
    /// </summary>
    /// <param name="request">Login request</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Login result</returns>
    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest request, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var result = await _authenticationService.SignInAsync(
            request.Email,
            request.Password,
            request.RememberMe,
            cancellationToken);

        if (result.Success)
        {
            return Ok(new
            {
                message = "Login successful",
                accessToken = result.AccessToken,
                expiresAt = result.ExpiresAt
            });
        }

        return Unauthorized(new { message = result.ErrorMessage });
    }

    /// <summary>
    /// User logout endpoint
    /// </summary>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Logout result</returns>
    [HttpPost("logout")]
    [Authorize]
    public async Task<IActionResult> Logout(CancellationToken cancellationToken)
    {
        await _authenticationService.SignOutAsync(cancellationToken);
        return Ok(new { message = "Logout successful" });
    }

    /// <summary>
    /// User registration endpoint
    /// </summary>
    /// <param name="request">Registration request</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Registration result</returns>
    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterRequest request, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var user = new ApplicationUser
        {
            UserName = request.Email,
            Email = request.Email,
            FirstName = request.FirstName,
            LastName = request.LastName,
            EmailConfirmed = true // For demo purposes, auto-confirm email
        };

        var (success, errors) = await _userService.CreateUserAsync(user, request.Password, cancellationToken);

        if (success)
        {
            return Ok(new { message = "Registration successful" });
        }

        return BadRequest(new { errors });
    }

    /// <summary>
    /// Get current user profile
    /// </summary>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>User profile</returns>
    [HttpGet("profile")]
    [Authorize]
    public async Task<IActionResult> GetProfile(CancellationToken cancellationToken)
    {
        var userId = _authenticationService.GetCurrentUserId();
        if (userId == null)
            return Unauthorized();

        var user = await _userService.GetUserByIdAsync(userId.Value, cancellationToken);
        if (user == null)
            return NotFound();

        return Ok(new
        {
            id = user.Id,
            email = user.Email,
            firstName = user.FirstName,
            lastName = user.LastName,
            fullName = user.FullName,
            isActive = user.IsActive,
            createdAt = user.CreatedAt,
            lastLoginAt = user.LastLoginAt
        });
    }

    /// <summary>
    /// Update user profile
    /// </summary>
    /// <param name="request">Update profile request</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Update result</returns>
    [HttpPut("profile")]
    [Authorize]
    public async Task<IActionResult> UpdateProfile([FromBody] UpdateProfileRequest request, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var userId = _authenticationService.GetCurrentUserId();
        if (userId == null)
            return Unauthorized();

        var user = await _userService.GetUserByIdAsync(userId.Value, cancellationToken);
        if (user == null)
            return NotFound();

        user.FirstName = request.FirstName;
        user.LastName = request.LastName;

        var (success, errors) = await _userService.UpdateUserAsync(user, cancellationToken);

        if (success)
        {
            return Ok(new { message = "Profile updated successfully" });
        }

        return BadRequest(new { errors });
    }

    /// <summary>
    /// Change user password
    /// </summary>
    /// <param name="request">Change password request</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Change password result</returns>
    [HttpPost("change-password")]
    [Authorize]
    public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordRequest request, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var userId = _authenticationService.GetCurrentUserId();
        if (userId == null)
            return Unauthorized();

        var (success, errors) = await _userService.ChangePasswordAsync(
            userId.Value,
            request.CurrentPassword,
            request.NewPassword,
            cancellationToken);

        if (success)
        {
            return Ok(new { message = "Password changed successfully" });
        }

        return BadRequest(new { errors });
    }
}

/// <summary>
/// Login request model
/// </summary>
public class LoginRequest
{
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public bool RememberMe { get; set; } = false;
}

/// <summary>
/// Registration request model
/// </summary>
public class RegisterRequest
{
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
}

/// <summary>
/// Update profile request model
/// </summary>
public class UpdateProfileRequest
{
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
}

/// <summary>
/// Change password request model
/// </summary>
public class ChangePasswordRequest
{
    public string CurrentPassword { get; set; } = string.Empty;
    public string NewPassword { get; set; } = string.Empty;
}