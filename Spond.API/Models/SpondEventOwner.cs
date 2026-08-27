using static Spond.API.Enums;

namespace Spond.API.Models;

/// <summary>
/// Represents an owner (organizer) of an event in the Spond system.
/// </summary>
public class SpondEventOwner : SpondMember
{
    /// <summary>
    /// The response status of the event owner.
    /// </summary>
    public EventResponse? Response { get; set; }
}