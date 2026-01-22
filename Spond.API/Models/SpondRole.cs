using static Spond.API.Enums;

namespace Spond.API.Models;

/// <summary>
/// Represents a role with specific permissions in the Spond system.
/// </summary>
public class SpondRole
{
    /// <summary>
    /// The unique identifier of the role.
    /// </summary>
    public string Id { get; set; } = string.Empty;
    
    /// <summary>
    /// The name of the role.
    /// </summary>
    public string Name { get; set; } = string.Empty;
    
    /// <summary>
    /// The list of permissions granted to this role.
    /// </summary>
    public List<Permission> Permissions { get; set; } = [];
}
