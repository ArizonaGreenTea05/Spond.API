namespace Spond.API.Models;

/// <summary>
/// Represents login credentials for authenticating with the Spond API.
/// </summary>
public class SpondLoginInformation
{
    /// <summary>
    /// The email address for login.
    /// </summary>
    public string Email { get; set; } = string.Empty;
    
    /// <summary>
    /// The password for login.
    /// </summary>
    public string Password { get; set; } = string.Empty;
}
