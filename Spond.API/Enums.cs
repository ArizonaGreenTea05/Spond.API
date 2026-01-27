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
    public enum EventVisibility
    {
        /// <summary>
        /// Visibility is undefined or not set.
        /// </summary>
        Undefined,
        /// <summary>
        /// Event is visible only to invitees.
        /// </summary>
        Invitees
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
