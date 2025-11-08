using Microsoft.AspNetCore.Identity;

namespace AscendID_API.Domain.Entities.Identity;

/// <summary>
/// Represents a role in the AscendID system
/// </summary>
public class ApplicationRole : IdentityRole<Guid>
{
    /// <summary>
    /// Gets or sets the description of the role
    /// </summary>
    public string? Description { get; set; }
    
    /// <summary>
    /// Gets or sets the date when the role was created
    /// </summary>
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    
    /// <summary>
    /// Gets or sets the date when the role was last updated
    /// </summary>
    public DateTime? UpdatedAt { get; set; }
    
    /// <summary>
    /// Gets or sets a value indicating whether the role is active
    /// </summary>
    public bool IsActive { get; set; } = true;
    
    /// <summary>
    /// Gets or sets a value indicating whether this is a system role that cannot be deleted
    /// </summary>
    public bool IsSystemRole { get; set; } = false;
}