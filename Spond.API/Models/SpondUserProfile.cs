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

    /// <summary>
    /// The email address of the user.
    /// </summary>
    public string? Email { get; set; }

    /// <summary>
    /// The phone number of the user.
    /// </summary>
    public string? PhoneNumber { get; set; }

    /// <summary>
    /// Defines the method the user prefers to be contacted.
    /// </summary>
    public Enums.ContactMethod ContactMethod { get; set; } = Enums.ContactMethod.Undefined;

    /// <summary>
    /// The status indicating whether the user is currently unreachable.
    /// </summary>
    public bool UnableToReach { get; set; }
}