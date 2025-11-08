using AscendID_API.Domain.Entities.Identity;

namespace AscendID_API.Application.Interfaces.Identity;

/// <summary>
/// Service interface for user management operations
/// </summary>
public interface IUserService
{
    /// <summary>
    /// Gets a user by their ID
    /// </summary>
    /// <param name="userId">The user ID</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The user if found, null otherwise</returns>
    Task<ApplicationUser?> GetUserByIdAsync(Guid userId, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Gets a user by their email address
    /// </summary>
    /// <param name="email">The email address</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The user if found, null otherwise</returns>
    Task<ApplicationUser?> GetUserByEmailAsync(string email, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Gets a user by their username
    /// </summary>
    /// <param name="username">The username</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The user if found, null otherwise</returns>
    Task<ApplicationUser?> GetUserByUsernameAsync(string username, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Creates a new user
    /// </summary>
    /// <param name="user">The user to create</param>
    /// <param name="password">The user's password</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>True if successful, false otherwise</returns>
    Task<(bool Success, IEnumerable<string> Errors)> CreateUserAsync(ApplicationUser user, string password, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Updates an existing user
    /// </summary>
    /// <param name="user">The user to update</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>True if successful, false otherwise</returns>
    Task<(bool Success, IEnumerable<string> Errors)> UpdateUserAsync(ApplicationUser user, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Deletes a user (soft delete - sets IsActive to false)
    /// </summary>
    /// <param name="userId">The user ID</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>True if successful, false otherwise</returns>
    Task<bool> DeleteUserAsync(Guid userId, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Validates user credentials
    /// </summary>
    /// <param name="email">The email</param>
    /// <param name="password">The password</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>True if credentials are valid, false otherwise</returns>
    Task<bool> ValidateCredentialsAsync(string email, string password, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Changes a user's password
    /// </summary>
    /// <param name="userId">The user ID</param>
    /// <param name="currentPassword">The current password</param>
    /// <param name="newPassword">The new password</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>True if successful, false otherwise</returns>
    Task<(bool Success, IEnumerable<string> Errors)> ChangePasswordAsync(Guid userId, string currentPassword, string newPassword, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Resets a user's password
    /// </summary>
    /// <param name="email">The user's email</param>
    /// <param name="token">The password reset token</param>
    /// <param name="newPassword">The new password</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>True if successful, false otherwise</returns>
    Task<(bool Success, IEnumerable<string> Errors)> ResetPasswordAsync(string email, string token, string newPassword, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Generates a password reset token for a user
    /// </summary>
    /// <param name="email">The user's email</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The password reset token if successful, null otherwise</returns>
    Task<string?> GeneratePasswordResetTokenAsync(string email, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Updates the user's last login timestamp
    /// </summary>
    /// <param name="userId">The user ID</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Task</returns>
    Task UpdateLastLoginAsync(Guid userId, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Gets users with pagination
    /// </summary>
    /// <param name="pageNumber">Page number (1-based)</param>
    /// <param name="pageSize">Page size</param>
    /// <param name="searchTerm">Optional search term</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Paginated list of users</returns>
    Task<(IEnumerable<ApplicationUser> Users, int TotalCount)> GetUsersAsync(int pageNumber, int pageSize, string? searchTerm = null, CancellationToken cancellationToken = default);
}