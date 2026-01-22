using static Spond.API.Enums;

namespace Spond.API.Models;

/// <summary>
/// Represents a group in the Spond system.
/// </summary>
public class SpondGroup
{
    /// <summary>
    /// The unique identifier of the group.
    /// </summary>
    public string Id { get; set; } = string.Empty;
    
    /// <summary>
    /// The name of the group.
    /// </summary>
    public string Name { get; set; } = string.Empty;
    
    /// <summary>
    /// The country code for the group's location.
    /// </summary>
    public string CountryCode { get; set; } = string.Empty;
    
    /// <summary>
    /// The URL for signing up to join the group.
    /// </summary>
    public string SignupUrl { get; set; } = string.Empty;
    
    /// <summary>
    /// The visibility level for events in the group.
    /// </summary>
    public EventVisibility EventVisibility { get; set; } = Enums.EventVisibility.Undefined;
    
    /// <summary>
    /// Indicates whether contact information should be shared among group members.
    /// </summary>
    public bool ShareContactInfo { get; set; }
    
    /// <summary>
    /// Indicates whether administrators can add members to the group.
    /// </summary>
    public bool AdminsCanAddMembers { get; set; }
    
    /// <summary>
    /// Indicates whether contact information is hidden from group members.
    /// </summary>
    public bool ContactInfoHidden { get; set; }
    
    /// <summary>
    /// The list of members in the group.
    /// </summary>
    public List<SpondMember> Members { get; set; } = [];
    
    /// <summary>
    /// The list of subgroups within the group.
    /// </summary>
    public List<SpondSubGroup> SubGroups { get; set; } = [];
    
    /// <summary>
    /// The list of roles defined for the group.
    /// </summary>
    public List<SpondRole> Roles { get; set; } = [];
    
    /// <summary>
    /// The format for displaying addresses in the group.
    /// </summary>
    public List<string> AddressFormat { get; set; } = [];
}