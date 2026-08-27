namespace Spond.API.Models;

/// <summary>
/// Represents the location of an event in the Spond system.
/// </summary>
public class SpondEventLocation
{
    /// <summary>
    /// The unique identifier of the location.
    /// </summary>
    public string? Id { get; set; }

    /// <summary>
    /// The feature name or venue name for the location.
    /// </summary>
    public string? Feature { get; set; }

    /// <summary>
    /// The street address of the location.
    /// </summary>
    public string? Address { get; set; }

    /// <summary>
    /// The latitude coordinate of the location.
    /// </summary>
    public double? Latitude { get; set; }

    /// <summary>
    /// The longitude coordinate of the location.
    /// </summary>
    public double? Longitude { get; set; }
}
