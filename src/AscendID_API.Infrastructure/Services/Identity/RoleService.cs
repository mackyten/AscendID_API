using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using AscendID_API.Application.Interfaces.Identity;
using AscendID_API.Domain.Entities.Identity;
using AscendID_API.Infrastructure.Data;

namespace AscendID_API.Infrastructure.Services.Identity;

/// <summary>
/// Implementation of role service for managing role operations
/// </summary>
public class RoleService : IRoleService
{
    private readonly RoleManager<ApplicationRole> _roleManager;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly ApplicationDbContext _context;

    public RoleService(RoleManager<ApplicationRole> roleManager, UserManager<ApplicationUser> userManager, ApplicationDbContext context)
    {
        _roleManager = roleManager;
        _userManager = userManager;
        _context = context;
    }

    public async Task<ApplicationRole?> GetRoleByIdAsync(Guid roleId, CancellationToken cancellationToken = default)
    {
        return await _roleManager.FindByIdAsync(roleId.ToString());
    }

    public async Task<ApplicationRole?> GetRoleByNameAsync(string roleName, CancellationToken cancellationToken = default)
    {
        return await _roleManager.FindByNameAsync(roleName);
    }

    public async Task<IEnumerable<ApplicationRole>> GetAllRolesAsync(CancellationToken cancellationToken = default)
    {
        return await _context.Roles
            .Where(r => r.IsActive)
            .OrderBy(r => r.Name)
            .ToListAsync(cancellationToken);
    }

    public async Task<(bool Success, IEnumerable<string> Errors)> CreateRoleAsync(ApplicationRole role, CancellationToken cancellationToken = default)
    {
        role.CreatedAt = DateTime.UtcNow;
        
        var result = await _roleManager.CreateAsync(role);
        
        return (result.Succeeded, result.Errors.Select(e => e.Description));
    }

    public async Task<(bool Success, IEnumerable<string> Errors)> UpdateRoleAsync(ApplicationRole role, CancellationToken cancellationToken = default)
    {
        role.UpdatedAt = DateTime.UtcNow;
        
        var result = await _roleManager.UpdateAsync(role);
        
        return (result.Succeeded, result.Errors.Select(e => e.Description));
    }

    public async Task<bool> DeleteRoleAsync(Guid roleId, CancellationToken cancellationToken = default)
    {
        var role = await _roleManager.FindByIdAsync(roleId.ToString());
        if (role == null)
            return false;

        // Prevent deletion of system roles
        if (role.IsSystemRole)
            return false;

        // Soft delete - set IsActive to false instead of deleting
        role.IsActive = false;
        role.UpdatedAt = DateTime.UtcNow;
        
        var result = await _roleManager.UpdateAsync(role);
        return result.Succeeded;
    }

    public async Task<(bool Success, IEnumerable<string> Errors)> AddUserToRoleAsync(Guid userId, string roleName, CancellationToken cancellationToken = default)
    {
        var user = await _userManager.FindByIdAsync(userId.ToString());
        if (user == null)
            return (false, new[] { "User not found" });

        var role = await _roleManager.FindByNameAsync(roleName);
        if (role == null || !role.IsActive)
            return (false, new[] { "Role not found or inactive" });

        var result = await _userManager.AddToRoleAsync(user, roleName);
        
        return (result.Succeeded, result.Errors.Select(e => e.Description));
    }

    public async Task<(bool Success, IEnumerable<string> Errors)> RemoveUserFromRoleAsync(Guid userId, string roleName, CancellationToken cancellationToken = default)
    {
        var user = await _userManager.FindByIdAsync(userId.ToString());
        if (user == null)
            return (false, new[] { "User not found" });

        var result = await _userManager.RemoveFromRoleAsync(user, roleName);
        
        return (result.Succeeded, result.Errors.Select(e => e.Description));
    }

    public async Task<IEnumerable<string>> GetUserRolesAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        var user = await _userManager.FindByIdAsync(userId.ToString());
        if (user == null)
            return Enumerable.Empty<string>();

        return await _userManager.GetRolesAsync(user);
    }

    public async Task<bool> IsUserInRoleAsync(Guid userId, string roleName, CancellationToken cancellationToken = default)
    {
        var user = await _userManager.FindByIdAsync(userId.ToString());
        if (user == null)
            return false;

        return await _userManager.IsInRoleAsync(user, roleName);
    }

    public async Task<IEnumerable<ApplicationUser>> GetUsersInRoleAsync(string roleName, CancellationToken cancellationToken = default)
    {
        var users = await _userManager.GetUsersInRoleAsync(roleName);
        return users.Where(u => u.IsActive);
    }
}