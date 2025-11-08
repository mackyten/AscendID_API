using AscendID_API.Domain.Entities.Identity;

namespace AscendID_API.Application.Interfaces.Identity;

/// <summary>
/// Service interface for role management operations
/// </summary>
public interface IRoleService
{
    /// <summary>
    /// Gets a role by its ID
    /// </summary>
    /// <param name="roleId">The role ID</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The role if found, null otherwise</returns>
    Task<ApplicationRole?> GetRoleByIdAsync(Guid roleId, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Gets a role by its name
    /// </summary>
    /// <param name="roleName">The role name</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The role if found, null otherwise</returns>
    Task<ApplicationRole?> GetRoleByNameAsync(string roleName, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Gets all roles
    /// </summary>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>List of all roles</returns>
    Task<IEnumerable<ApplicationRole>> GetAllRolesAsync(CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Creates a new role
    /// </summary>
    /// <param name="role">The role to create</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>True if successful, false otherwise</returns>
    Task<(bool Success, IEnumerable<string> Errors)> CreateRoleAsync(ApplicationRole role, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Updates an existing role
    /// </summary>
    /// <param name="role">The role to update</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>True if successful, false otherwise</returns>
    Task<(bool Success, IEnumerable<string> Errors)> UpdateRoleAsync(ApplicationRole role, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Deletes a role (soft delete for non-system roles)
    /// </summary>
    /// <param name="roleId">The role ID</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>True if successful, false otherwise</returns>
    Task<bool> DeleteRoleAsync(Guid roleId, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Assigns a role to a user
    /// </summary>
    /// <param name="userId">The user ID</param>
    /// <param name="roleName">The role name</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>True if successful, false otherwise</returns>
    Task<(bool Success, IEnumerable<string> Errors)> AddUserToRoleAsync(Guid userId, string roleName, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Removes a role from a user
    /// </summary>
    /// <param name="userId">The user ID</param>
    /// <param name="roleName">The role name</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>True if successful, false otherwise</returns>
    Task<(bool Success, IEnumerable<string> Errors)> RemoveUserFromRoleAsync(Guid userId, string roleName, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Gets all roles assigned to a user
    /// </summary>
    /// <param name="userId">The user ID</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>List of role names</returns>
    Task<IEnumerable<string>> GetUserRolesAsync(Guid userId, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Checks if a user is in a specific role
    /// </summary>
    /// <param name="userId">The user ID</param>
    /// <param name="roleName">The role name</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>True if user is in role, false otherwise</returns>
    Task<bool> IsUserInRoleAsync(Guid userId, string roleName, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Gets all users in a specific role
    /// </summary>
    /// <param name="roleName">The role name</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>List of users in the role</returns>
    Task<IEnumerable<ApplicationUser>> GetUsersInRoleAsync(string roleName, CancellationToken cancellationToken = default);
}