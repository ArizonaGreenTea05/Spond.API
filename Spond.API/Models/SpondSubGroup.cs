namespace Spond.API.Models;

/// <summary>
/// Represents a subgroup within a Spond group.
/// </summary>
public class SpondSubGroup
{
    /// <summary>
    /// The unique identifier of the subgroup.
    /// </summary>
    public string Id { get; set; } = string.Empty;
    
    /// <summary>
    /// The name of the subgroup.
    /// </summary>
    public string Name { get; set; } = string.Empty;
    
    /// <summary>
    /// The color associated with the subgroup for visual identification.
    /// </summary>
    public string Color { get; set; } = string.Empty;
}
