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

    /// <summary>
    /// A description or additional details for the event.
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// The type of Spond entry (e.g. "EVENT").
    /// </summary>
    public string? SpondType { get; set; }

    /// <summary>
    /// Indicates whether comments are disabled for this event.
    /// </summary>
    public bool? CommentsDisabled { get; set; }

    /// <summary>
    /// The maximum number of accepted responses. 0 means no limit.
    /// </summary>
    public int? MaxAccepted { get; set; }

    /// <summary>
    /// The RSVP deadline as an ISO 8601 timestamp string.
    /// </summary>
    public string? RsvpDate { get; set; }

    /// <summary>
    /// The location details for the event.
    /// </summary>
    public SpondEventLocation? Location { get; set; }

    /// <summary>
    /// The visibility setting for the event (e.g. "INVITEES").
    /// </summary>
    public string? Visibility { get; set; }

    /// <summary>
    /// Indicates whether the participant list is hidden from invitees.
    /// </summary>
    public bool? ParticipantsHidden { get; set; }

    /// <summary>
    /// The auto-reminder type for the event (e.g. "DISABLED").
    /// </summary>
    public string? AutoReminderType { get; set; }

    /// <summary>
    /// Indicates whether responses are automatically accepted.
    /// </summary>
    public bool? AutoAccept { get; set; }

    /// <summary>
    /// The tasks associated with this event.
    /// </summary>
    public SpondEventTasks? Tasks { get; set; }

    /// <summary>
    /// The comments on this event.
    /// </summary>
    public List<SpondComment> Comments { get; set; } = [];
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

    /// <summary>
    /// The list of member IDs who have declined the event invitation.
    /// </summary>
    public List<string> DeclinedIds { get; set; } = [];

    /// <summary>
    /// The list of member IDs who have not yet responded to the event invitation.
    /// </summary>
    public List<string> UnansweredIds { get; set; } = [];

    /// <summary>
    /// The list of member IDs on the waiting list for the event.
    /// </summary>
    public List<string> WaitinglistIds { get; set; } = [];

    /// <summary>
    /// The list of member IDs with unconfirmed responses.
    /// </summary>
    public List<string> UnconfirmedIds { get; set; } = [];

    /// <summary>
    /// A dictionary mapping member IDs to their decline messages.
    /// </summary>
    public Dictionary<string, string> DeclineMessages { get; set; } = [];
}