using Newtonsoft.Json;

namespace Spond.API.Models;

/// <summary>
/// Represents a member in the Spond system.
/// </summary>
public class SpondMember
{
    /// <summary>
    /// The unique identifier of the member.
    /// </summary>
    public string Id { get; set; } = string.Empty;
    
    /// <summary>
    /// The user profile associated with the member.
    /// </summary>
    public SpondUserProfile? Profile { get; set; }
    
    /// <summary>
    /// The first name of the member.
    /// </summary>
    public string FirstName { get; set; } = string.Empty;
    
    /// <summary>
    /// The last name of the member.
    /// </summary>
    public string LastName { get; set; } = string.Empty;
    
    /// <summary>
    /// The timestamp when the member was created.
    /// </summary>
    public string CreatedTime { get; set; } = string.Empty;
    
    /// <summary>
    /// The date of birth of the member as a string.
    /// </summary>
    public string? DateOfBirth { get; set; }

    /// <summary>
    /// The date of birth of the member as a DateTime object.
    /// </summary>
    [JsonIgnore]
    public DateTime? Birthday => DateTime.TryParse(DateOfBirth, out var dateTime) ? dateTime : null;

    /// <summary>
    /// The status whether the date of birth has been verified.
    /// </summary>
    public bool VerifiedDateOfBirth { get; set; }

    /// <summary>
    /// The type of role the member has.
    /// </summary>
    public bool Respondent { get; set; }

    /// <summary>
    /// The email address of the member.
    /// </summary>
    public string? Email { get; set; }

    /// <summary>
    /// The phone number of the member.
    /// </summary>
    public string? PhoneNumber { get; set; }

    /// <summary>
    /// The url of the member's profile image.
    /// </summary>
    public string? ImageUrl { get; set; }

    /// <summary>
    /// The list of guardians for the member (typically for minor members).
    /// </summary>
    public List<SpondMember> Guardians { get; set; } = [];

    /// <summary>
    /// The list of IDs of the subgroups the member is part of.
    /// </summary>
    public List<string> SubGroups { get; set; } = [];
}