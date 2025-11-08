namespace AscendID_API.Domain.Entities.Identity;

/// <summary>
/// Represents an OAuth scope that defines access permissions
/// </summary>
public class OAuthScope
{
    /// <summary>
    /// Gets or sets the unique identifier for the scope
    /// </summary>
    public Guid Id { get; set; }
    
    /// <summary>
    /// Gets or sets the name of the scope
    /// </summary>
    public string Name { get; set; } = string.Empty;
    
    /// <summary>
    /// Gets or sets the display name of the scope
    /// </summary>
    public string DisplayName { get; set; } = string.Empty;
    
    /// <summary>
    /// Gets or sets the description of the scope
    /// </summary>
    public string? Description { get; set; }
    
    /// <summary>
    /// Gets or sets a value indicating whether the scope is enabled
    /// </summary>
    public bool IsEnabled { get; set; } = true;
    
    /// <summary>
    /// Gets or sets a value indicating whether user consent is required for this scope
    /// </summary>
    public bool RequireConsent { get; set; } = false;
    
    /// <summary>
    /// Gets or sets a value indicating whether this scope should be emphasized in consent screens
    /// </summary>
    public bool IsEmphasized { get; set; } = false;
    
    /// <summary>
    /// Gets or sets a value indicating whether this scope is a system scope
    /// </summary>
    public bool IsSystemScope { get; set; } = false;
    
    /// <summary>
    /// Gets or sets the date when the scope was created
    /// </summary>
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    
    /// <summary>
    /// Gets or sets the date when the scope was last updated
    /// </summary>
    public DateTime? UpdatedAt { get; set; }
}