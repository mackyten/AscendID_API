using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using AscendID_API.Application.Interfaces.Identity;
using AscendID_API.Domain.Entities.Identity;
using AscendID_API.Infrastructure.Data;

namespace AscendID_API.Infrastructure.Services.Identity;

/// <summary>
/// Implementation of user service for managing user operations
/// </summary>
public class UserService : IUserService
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly ApplicationDbContext _context;

    public UserService(UserManager<ApplicationUser> userManager, ApplicationDbContext context)
    {
        _userManager = userManager;
        _context = context;
    }

    public async Task<ApplicationUser?> GetUserByIdAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        return await _userManager.FindByIdAsync(userId.ToString());
    }

    public async Task<ApplicationUser?> GetUserByEmailAsync(string email, CancellationToken cancellationToken = default)
    {
        return await _userManager.FindByEmailAsync(email);
    }

    public async Task<ApplicationUser?> GetUserByUsernameAsync(string username, CancellationToken cancellationToken = default)
    {
        return await _userManager.FindByNameAsync(username);
    }

    public async Task<(bool Success, IEnumerable<string> Errors)> CreateUserAsync(ApplicationUser user, string password, CancellationToken cancellationToken = default)
    {
        user.CreatedAt = DateTime.UtcNow;
        user.SecurityStamp = Guid.NewGuid().ToString();
        
        var result = await _userManager.CreateAsync(user, password);
        
        return (result.Succeeded, result.Errors.Select(e => e.Description));
    }

    public async Task<(bool Success, IEnumerable<string> Errors)> UpdateUserAsync(ApplicationUser user, CancellationToken cancellationToken = default)
    {
        user.UpdatedAt = DateTime.UtcNow;
        
        var result = await _userManager.UpdateAsync(user);
        
        return (result.Succeeded, result.Errors.Select(e => e.Description));
    }

    public async Task<bool> DeleteUserAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        var user = await _userManager.FindByIdAsync(userId.ToString());
        if (user == null)
            return false;

        // Soft delete - set IsActive to false instead of deleting
        user.IsActive = false;
        user.UpdatedAt = DateTime.UtcNow;
        
        var result = await _userManager.UpdateAsync(user);
        return result.Succeeded;
    }

    public async Task<bool> ValidateCredentialsAsync(string email, string password, CancellationToken cancellationToken = default)
    {
        var user = await _userManager.FindByEmailAsync(email);
        if (user == null || !user.IsActive)
            return false;

        return await _userManager.CheckPasswordAsync(user, password);
    }

    public async Task<(bool Success, IEnumerable<string> Errors)> ChangePasswordAsync(Guid userId, string currentPassword, string newPassword, CancellationToken cancellationToken = default)
    {
        var user = await _userManager.FindByIdAsync(userId.ToString());
        if (user == null)
            return (false, new[] { "User not found" });

        var result = await _userManager.ChangePasswordAsync(user, currentPassword, newPassword);
        
        if (result.Succeeded)
        {
            user.UpdatedAt = DateTime.UtcNow;
            await _userManager.UpdateAsync(user);
        }
        
        return (result.Succeeded, result.Errors.Select(e => e.Description));
    }

    public async Task<(bool Success, IEnumerable<string> Errors)> ResetPasswordAsync(string email, string token, string newPassword, CancellationToken cancellationToken = default)
    {
        var user = await _userManager.FindByEmailAsync(email);
        if (user == null)
            return (false, new[] { "User not found" });

        var result = await _userManager.ResetPasswordAsync(user, token, newPassword);
        
        if (result.Succeeded)
        {
            user.UpdatedAt = DateTime.UtcNow;
            await _userManager.UpdateAsync(user);
        }
        
        return (result.Succeeded, result.Errors.Select(e => e.Description));
    }

    public async Task<string?> GeneratePasswordResetTokenAsync(string email, CancellationToken cancellationToken = default)
    {
        var user = await _userManager.FindByEmailAsync(email);
        if (user == null || !user.IsActive)
            return null;

        return await _userManager.GeneratePasswordResetTokenAsync(user);
    }

    public async Task UpdateLastLoginAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        var user = await _userManager.FindByIdAsync(userId.ToString());
        if (user != null)
        {
            user.LastLoginAt = DateTime.UtcNow;
            user.UpdatedAt = DateTime.UtcNow;
            await _userManager.UpdateAsync(user);
        }
    }

    public async Task<(IEnumerable<ApplicationUser> Users, int TotalCount)> GetUsersAsync(int pageNumber, int pageSize, string? searchTerm = null, CancellationToken cancellationToken = default)
    {
        var query = _context.Users.Where(u => u.IsActive);

        if (!string.IsNullOrWhiteSpace(searchTerm))
        {
            var search = searchTerm.ToLower();
            query = query.Where(u => 
                u.FirstName.ToLower().Contains(search) ||
                u.LastName.ToLower().Contains(search) ||
                u.Email!.ToLower().Contains(search) ||
                u.UserName!.ToLower().Contains(search));
        }

        var totalCount = await query.CountAsync(cancellationToken);
        
        var users = await query
            .OrderBy(u => u.FirstName)
            .ThenBy(u => u.LastName)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return (users, totalCount);
    }
}