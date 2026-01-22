namespace Spond.API.Models;

/// <summary>
/// Represents a user profile in the Spond system.
/// </summary>
public class SpondUserProfile
{
    /// <summary>
    /// The unique identifier of the user profile.
    /// </summary>
    public string Id { get; set; } = string.Empty;
    
    /// <summary>
    /// The first name of the user.
    /// </summary>
    public string FirstName { get; set; } = string.Empty;
    
    /// <summary>
    /// The last name of the user.
    /// </summary>
    public string LastName { get; set; } = string.Empty;
}