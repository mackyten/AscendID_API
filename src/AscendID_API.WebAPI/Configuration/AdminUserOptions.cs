namespace AscendID_API.WebAPI.Configuration;

/// <summary>
/// Configuration options for the admin user seeding
/// </summary>
public class AdminUserOptions
{
    /// <summary>
    /// Configuration section name
    /// </summary>
    public const string SectionName = "AdminUser";
    
    /// <summary>
    /// Admin user email address
    /// </summary>
    public string Email { get; set; } = string.Empty;
    
    /// <summary>
    /// Admin user password
    /// </summary>
    public string Password { get; set; } = string.Empty;
}