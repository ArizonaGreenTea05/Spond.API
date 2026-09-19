namespace Spond.API.Models;

using static Spond.API.Enums;

/// <summary>
/// Represents an update request for an existing Spond event.
/// Only populate the properties you wish to change; null values are ignored
/// and the current event's values are preserved.
/// </summary>
public class SpondEventUpdateRequest
{
    /// <summary>
    /// The new heading/title of the event.
    /// </summary>
    public string? Heading { get; set; }

    /// <summary>
    /// The new description of the event.
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// The new start time of the event as an ISO 8601 timestamp string.
    /// </summary>
    public string? StartTimestamp { get; set; }

    /// <summary>
    /// The new end time of the event as an ISO 8601 timestamp string.
    /// </summary>
    public string? EndTimestamp { get; set; }

    /// <summary>
    /// Whether to disable comments for the event.
    /// </summary>
    public bool? CommentsDisabled { get; set; }

    /// <summary>
    /// The maximum number of accepted responses. Use 0 for no limit.
    /// </summary>
    public int? MaxAccepted { get; set; }

    /// <summary>
    /// The RSVP deadline as an ISO 8601 timestamp string.
    /// </summary>
    public string? RsvpDate { get; set; }

    /// <summary>
    /// The updated location details for the event.
    /// </summary>
    public SpondEventLocation? Location { get; set; }

    /// <summary>
    /// The visibility setting for the event.
    /// </summary>
    public EventVisibility? Visibility { get; set; }

    /// <summary>
    /// Whether to hide the participant list from invitees.
    /// </summary>
    public bool? ParticipantsHidden { get; set; }

    /// <summary>
    /// The auto-reminder type for the event.
    /// </summary>
    public AutoReminderType? AutoReminderType { get; set; }

    /// <summary>
    /// Whether to automatically accept all responses.
    /// </summary>
    public bool? AutoAccept { get; set; }
}
