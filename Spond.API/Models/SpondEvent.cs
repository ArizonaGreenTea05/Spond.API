using Spond.API.Extensions;
using static Spond.API.Extensions.DateTimeExtensions;

namespace Spond.API.Models;

/// <summary>
/// Represents an event in the Spond system.
/// </summary>
public class SpondEvent
{
    /// <summary>
    /// The unique identifier of the event.
    /// </summary>
    public string Id { get; set; } = string.Empty;
    
    /// <summary>
    /// The heading or title of the event.
    /// </summary>
    public string Heading { get; set; } = string.Empty;
    
    /// <summary>
    /// The name of the event (alias for Heading).
    /// </summary>
    public string Name => Heading;
    
    /// <summary>
    /// The start time of the event.
    /// </summary>
    public DateTime StartTime { get; set; }
    
    /// <summary>
    /// The end time of the event.
    /// </summary>
    public DateTime EndTime { get; set; }

    /// <summary>
    /// The start time of the event as an ISO 8601 timestamp string.
    /// </summary>
    public string StartTimestamp
    {
        get => StartTime.ToIso8601(false);
        set => StartTime = FromIso8601(value, false);
    }

    /// <summary>
    /// The end time of the event as an ISO 8601 timestamp string.
    /// </summary>
    public string EndTimestamp
    {
        get => EndTime.ToIso8601(false);
        set => EndTime = FromIso8601(value, false);
    }

    /// <summary>
    /// The list of owners (organizers) of the event.
    /// </summary>
    public List<SpondEventOwner> Owners { get; set; } = [];

    /// <summary>
    /// The list of owners who have accepted the event invitation.
    /// </summary>
    public List<SpondEventOwner> AcceptedOwners => Owners.Where(o => o.Response == "accepted").ToList();

    /// <summary>
    /// The list of members who have accepted the event invitation.
    /// </summary>
    public List<SpondMember> AcceptedMembers => Responses?.AcceptedIds
        .Select(id => Recipients?.Group?.Members.FirstOrDefault(m => m.Id == id)).Where(m => m is not null).Cast<SpondMember>().ToList() ?? [];

    /// <summary>
    /// The recipients of the event invitation.
    /// </summary>
    public SpondEventRecipients? Recipients { get; set; }

    /// <summary>
    /// The responses to the event invitation.
    /// </summary>
    public SpondEventResponses? Responses { get; set; }

    /// <summary>
    /// Indicates whether the event has been cancelled.
    /// </summary>
    public bool? Cancelled { get; set; }
}

/// <summary>
/// Represents the recipients of an event invitation.
/// </summary>
public class SpondEventRecipients
{
    /// <summary>
    /// The group that received the event invitation.
    /// </summary>
    public SpondGroup? Group { get; set; }
}

/// <summary>
/// Represents the responses to an event invitation.
/// </summary>
public class SpondEventResponses
{
    /// <summary>
    /// The list of member IDs who have accepted the event invitation.
    /// </summary>
    public List<string> AcceptedIds { get; set; } = [];
}