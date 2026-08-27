using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System.Runtime.Serialization;

namespace Spond.API;

/// <summary>
/// Contains enumerations used throughout the Spond API.
/// </summary>
public static class Enums
{
    /// <summary>
    /// Defines the sort order for queries.
    /// </summary>
    public enum Order
    {
        /// <summary>
        /// Sort in ascending order.
        /// </summary>
        Ascending,
        /// <summary>
        /// Sort in descending order.
        /// </summary>
        Descending
    }

    /// <summary>
    /// Defines the visibility settings for events.
    /// </summary>
    [JsonConverter(typeof(StringEnumConverter))]
    public enum EventVisibility
    {
        /// <summary>
        /// Visibility is undefined or not set.
        /// </summary>
        Undefined,
        /// <summary>
        /// Event is visible only to invitees.
        /// </summary>
        [EnumMember(Value = "INVITEES")]
        Invitees,
        /// <summary>
        /// Event is visible to all group members.
        /// </summary>
        [EnumMember(Value = "GROUP")]
        Group,
        /// <summary>
        /// Event is publicly visible.
        /// </summary>
        [EnumMember(Value = "EVERYONE")]
        Everyone
    }

    /// <summary>
    /// Defines the type of a Spond entry.
    /// </summary>
    [JsonConverter(typeof(StringEnumConverter))]
    public enum SpondType
    {
        /// <summary>
        /// A standard one-time event.
        /// </summary>
        [EnumMember(Value = "EVENT")]
        Event,
        /// <summary>
        /// A recurring event series.
        /// </summary>
        [EnumMember(Value = "RECURRING")]
        Recurring
    }

    /// <summary>
    /// Defines the auto-reminder options for an event.
    /// </summary>
    [JsonConverter(typeof(StringEnumConverter))]
    public enum AutoReminderType
    {
        /// <summary>
        /// No automatic reminder is sent.
        /// </summary>
        [EnumMember(Value = "DISABLED")]
        Disabled,
        /// <summary>
        /// A reminder is sent 24 hours before the event.
        /// </summary>
        [EnumMember(Value = "HOURS_24")]
        Hours24,
        /// <summary>
        /// A reminder is sent 48 hours before the event.
        /// </summary>
        [EnumMember(Value = "HOURS_48")]
        Hours48,
        /// <summary>
        /// A reminder is sent 72 hours before the event.
        /// </summary>
        [EnumMember(Value = "HOURS_72")]
        Hours72
    }

    /// <summary>
    /// Defines the response status of a member to an event invitation.
    /// </summary>
    [JsonConverter(typeof(StringEnumConverter))]
    public enum EventResponse
    {
        /// <summary>
        /// The member has accepted the invitation.
        /// </summary>
        [EnumMember(Value = "accepted")]
        Accepted,
        /// <summary>
        /// The member has declined the invitation.
        /// </summary>
        [EnumMember(Value = "declined")]
        Declined,
        /// <summary>
        /// The member has not yet responded to the invitation.
        /// </summary>
        [EnumMember(Value = "unanswered")]
        Unanswered,
        /// <summary>
        /// The member is on the waiting list for the event.
        /// </summary>
        [EnumMember(Value = "waitinglist")]
        Waitinglist,
        /// <summary>
        /// The member's response is unconfirmed.
        /// </summary>
        [EnumMember(Value = "unconfirmed")]
        Unconfirmed
    }

    /// <summary>
    /// Defines the types of posts on group walls.
    /// </summary>
    [JsonConverter(typeof(StringEnumConverter))]
    public enum PostType
    {
        /// <summary>
        /// A plain text post.
        /// </summary>
        [EnumMember(Value = "PLAIN")]
        Plain
    }

    /// <summary>
    /// Defines the types of chat messages.
    /// </summary>
    [JsonConverter(typeof(StringEnumConverter))]
    public enum MessageType
    {
        /// <summary>
        /// A plain text message.
        /// </summary>
        [EnumMember(Value = "TEXT")]
        Text
    }

    /// <summary>
    /// Defines the types of tasks on an event.
    /// </summary>
    [JsonConverter(typeof(StringEnumConverter))]
    public enum TaskType
    {
        /// <summary>
        /// A task that has been assigned to specific members.
        /// </summary>
        [EnumMember(Value = "ASSIGNED")]
        Assigned,
        /// <summary>
        /// An open task that any member can pick up.
        /// </summary>
        [EnumMember(Value = "OPEN")]
        Open
    }

    /// <summary>
    /// Defines the method a member prefers to be contacted.
    /// </summary>
    public enum ContactMethod
    {
        /// <summary>
        /// Contact method is undefined or not set.
        /// </summary>
        Undefined,
        /// <summary>
        /// Contact via app.
        /// </summary>
        App,
        /// <summary>
        /// Contact via email.
        /// </summary>
        Email,
        /// <summary>
        /// Contact via phone.
        /// </summary>
        Phone
    }

    /// <summary>
    /// Defines the various permissions available in Spond groups.
    /// </summary>
    [JsonConverter(typeof(StringEnumConverter))]
    public enum Permission
    {
        /// <summary>
        /// Access to member management.
        /// </summary>
        Members,
        /// <summary>
        /// Administrator access.
        /// </summary>
        Admins,
        /// <summary>
        /// Access to settings.
        /// </summary>
        Settings,
        /// <summary>
        /// Access to events.
        /// </summary>
        Events,
        /// <summary>
        /// Access to posts.
        /// </summary>
        Posts,
        /// <summary>
        /// Access to polls.
        /// </summary>
        Polls,
        /// <summary>
        /// Access to payments.
        /// </summary>
        Payments,
        /// <summary>
        /// Access to chat functionality.
        /// </summary>
        Chat,
        /// <summary>
        /// Access to files.
        /// </summary>
        Files,
        /// <summary>
        /// Access to fundraisers.
        /// </summary>
        Fundraisers,
        /// <summary>
        /// Access to the coaches corner feature.
        /// </summary>
        [EnumMember(Value = "coaches-corner")]
        CoachesCorner
    }
}

