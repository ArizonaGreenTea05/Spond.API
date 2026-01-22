namespace Spond.API.Models;

/// <summary>
/// Represents an owner (organizer) of an event in the Spond system.
/// </summary>
public class SpondEventOwner : SpondMember
{
    /// <summary>
    /// The response status of the event owner (e.g., "accepted", "declined", "unanswered").
    /// </summary>
    public string? Response { get; set; }
}