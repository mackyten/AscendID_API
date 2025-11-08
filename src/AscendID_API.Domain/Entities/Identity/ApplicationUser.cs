using Microsoft.AspNetCore.Identity;

namespace AscendID_API.Domain.Entities.Identity;

/// <summary>
/// Represents a user in the AscendID system
/// </summary>
public class ApplicationUser : IdentityUser<Guid>
{
    /// <summary>
    /// Gets or sets the first name of the user
    /// </summary>
    public string FirstName { get; set; } = string.Empty;
    
    /// <summary>
    /// Gets or sets the last name of the user
    /// </summary>
    public string LastName { get; set; } = string.Empty;
    
    /// <summary>
    /// Gets or sets the full name of the user
    /// </summary>
    public string FullName => $"{FirstName} {LastName}".Trim();
    
    /// <summary>
    /// Gets or sets the date when the user was created
    /// </summary>
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    
    /// <summary>
    /// Gets or sets the date when the user was last updated
    /// </summary>
    public DateTime? UpdatedAt { get; set; }
    
    /// <summary>
    /// Gets or sets a value indicating whether the user is active
    /// </summary>
    public bool IsActive { get; set; } = true;
    
    /// <summary>
    /// Gets or sets the date when the user last logged in
    /// </summary>
    public DateTime? LastLoginAt { get; set; }
    
    /// <summary>
    /// Gets or sets the user's profile picture URL
    /// </summary>
    public string? ProfilePictureUrl { get; set; }
    
    /// <summary>
    /// Gets or sets additional user metadata as JSON
    /// </summary>
    public string? Metadata { get; set; }
}