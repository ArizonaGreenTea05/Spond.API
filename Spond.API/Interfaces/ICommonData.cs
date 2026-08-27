namespace Spond.API.Interfaces;

/// <summary>
/// Interface defining common data and URL construction methods for the Spond API.
/// </summary>
public interface ICommonData
{
    /// <summary>
    /// Gets the name of the login token property in the API response.
    /// </summary>
    string LoginTokenPropertyName { get; }

    /// <summary>
    /// Gets the name of the login token property in the API response if the token is wrapped in an object.
    /// </summary>
    string NestedLoginTokenPropertyName { get; }

    /// <summary>
    /// Gets the base URL for the Spond API.
    /// </summary>
    string BaseUrl { get; }
    
    /// <summary>
    /// Gets the URL endpoint for authentication.
    /// </summary>
    string LoginUrl { get; }
    
    /// <summary>
    /// Gets the URL endpoint for retrieving user profile information.
    /// </summary>
    string UserUrl { get; }
    
    /// <summary>
    /// Gets the URL endpoint for retrieving groups.
    /// </summary>
    string GroupsUrl { get; }

    /// <summary>
    /// Gets the URL endpoint for the chat server handshake.
    /// </summary>
    string ChatUrl { get; }

    /// <summary>
    /// Gets the URL endpoint for retrieving posts.
    /// </summary>
    /// <param name="max">Maximum number of posts to retrieve.</param>
    /// <param name="includeComments">Whether to include comments on posts.</param>
    /// <param name="groupId">Optional group ID to filter posts.</param>
    /// <param name="type">The type of posts to retrieve. Defaults to <see cref="Enums.PostType.Plain"/>.</param>
    /// <returns>A formatted URL string for the posts endpoint.</returns>
    string GetPostsUrl(int max, bool includeComments, string? groupId = null, Enums.PostType type = Enums.PostType.Plain);

    /// <summary>
    /// Gets the URL for retrieving or updating a single event by ID.
    /// </summary>
    /// <param name="eventId">The UID of the event.</param>
    /// <returns>A formatted URL string for the single event endpoint.</returns>
    string GetEventUrl(string eventId);

    /// <summary>
    /// Gets the URL for downloading the attendance XLSX export of an event.
    /// </summary>
    /// <param name="eventId">The UID of the event.</param>
    /// <returns>A formatted URL string for the event attendance export endpoint.</returns>
    string GetEventAttendanceUrl(string eventId);

    /// <summary>
    /// Gets the URL for changing a member's response to an event.
    /// </summary>
    /// <param name="eventId">The UID of the event.</param>
    /// <param name="userId">The member's ID.</param>
    /// <returns>A formatted URL string for the event response endpoint.</returns>
    string GetEventResponseUrl(string eventId, string userId);

    /// <summary>
    /// Constructs a URL for retrieving events across all groups within a time range.
    /// </summary>
    /// <param name="minEndTime">The minimum end time for events.</param>
    /// <param name="maxEndTime">The maximum end time for events.</param>
    /// <param name="includeComments">Whether to include event comments.</param>
    /// <param name="includeHidden">Whether to include hidden events.</param>
    /// <param name="addProfileInfo">Whether to add profile information.</param>
    /// <param name="scheduled">Whether to include scheduled events.</param>
    /// <param name="order">The sort order for events.</param>
    /// <param name="max">The maximum number of events to retrieve.</param>
    /// <returns>A formatted URL string for the events endpoint.</returns>
    string GetEventsUrl(DateTime minEndTime, DateTime maxEndTime, bool? includeComments, bool? includeHidden, bool? addProfileInfo, bool? scheduled, Enums.Order? order, int? max);

    /// <summary>
    /// Constructs a URL for retrieving events for a specific group within a time range.
    /// </summary>
    /// <param name="groupId">The ID of the group.</param>
    /// <param name="minEndTime">The minimum end time for events.</param>
    /// <param name="maxEndTime">The maximum end time for events.</param>
    /// <param name="includeComments">Whether to include event comments.</param>
    /// <param name="includeHidden">Whether to include hidden events.</param>
    /// <param name="addProfileInfo">Whether to add profile information.</param>
    /// <param name="scheduled">Whether to include scheduled events.</param>
    /// <param name="order">The sort order for events.</param>
    /// <param name="max">The maximum number of events to retrieve.</param>
    /// <returns>A formatted URL string for the events endpoint.</returns>
    string GetEventsUrl(string groupId, DateTime minEndTime, DateTime maxEndTime, bool? includeComments, bool? includeHidden, bool? addProfileInfo, bool? scheduled, Enums.Order? order, int? max);

    /// <summary>
    /// Constructs a URL for retrieving events for a specific subgroup within a time range.
    /// </summary>
    /// <param name="groupId">The ID of the parent group.</param>
    /// <param name="subGroupId">The ID of the subgroup.</param>
    /// <param name="minEndTime">The minimum end time for events.</param>
    /// <param name="maxEndTime">The maximum end time for events.</param>
    /// <param name="includeComments">Whether to include event comments.</param>
    /// <param name="includeHidden">Whether to include hidden events.</param>
    /// <param name="addProfileInfo">Whether to add profile information.</param>
    /// <param name="scheduled">Whether to include scheduled events.</param>
    /// <param name="order">The sort order for events.</param>
    /// <param name="max">The maximum number of events to retrieve.</param>
    /// <returns>A formatted URL string for the events endpoint.</returns>
    string GetEventsUrl(string groupId, string subGroupId, DateTime minEndTime, DateTime maxEndTime, bool? includeComments, bool? includeHidden, bool? addProfileInfo, bool? scheduled, Enums.Order? order, int? max);

    /// <summary>
    /// Constructs a URL for retrieving events across all groups with optional start and end time filters.
    /// </summary>
    /// <param name="minEndTime">The minimum end time for events.</param>
    /// <param name="maxEndTime">The maximum end time for events.</param>
    /// <param name="minStartTime">The minimum start time for events.</param>
    /// <param name="maxStartTime">The maximum start time for events.</param>
    /// <param name="includeComments">Whether to include event comments.</param>
    /// <param name="includeHidden">Whether to include hidden events.</param>
    /// <param name="addProfileInfo">Whether to add profile information.</param>
    /// <param name="scheduled">Whether to include scheduled events.</param>
    /// <param name="order">The sort order for events.</param>
    /// <param name="max">The maximum number of events to retrieve.</param>
    /// <returns>A formatted URL string for the events endpoint.</returns>
    string GetEventsUrl(DateTime? minEndTime, DateTime? maxEndTime, DateTime? minStartTime, DateTime? maxStartTime, bool? includeComments, bool? includeHidden, bool? addProfileInfo, bool? scheduled, Enums.Order? order, int? max);

    /// <summary>
    /// Constructs a URL for retrieving events for a specific group with optional start and end time filters.
    /// </summary>
    /// <param name="groupId">The ID of the group.</param>
    /// <param name="minEndTime">The minimum end time for events.</param>
    /// <param name="maxEndTime">The maximum end time for events.</param>
    /// <param name="minStartTime">The minimum start time for events.</param>
    /// <param name="maxStartTime">The maximum start time for events.</param>
    /// <param name="includeComments">Whether to include event comments.</param>
    /// <param name="includeHidden">Whether to include hidden events.</param>
    /// <param name="addProfileInfo">Whether to add profile information.</param>
    /// <param name="scheduled">Whether to include scheduled events.</param>
    /// <param name="order">The sort order for events.</param>
    /// <param name="max">The maximum number of events to retrieve.</param>
    /// <returns>A formatted URL string for the events endpoint.</returns>
    string GetEventsUrl(string groupId, DateTime? minEndTime, DateTime? maxEndTime, DateTime? minStartTime, DateTime? maxStartTime, bool? includeComments, bool? includeHidden, bool? addProfileInfo, bool? scheduled, Enums.Order? order, int? max);

    /// <summary>
    /// Constructs a URL for retrieving events for a specific subgroup with optional start and end time filters.
    /// </summary>
    /// <param name="groupId">The ID of the parent group.</param>
    /// <param name="subGroupId">The ID of the subgroup.</param>
    /// <param name="minEndTime">The minimum end time for events.</param>
    /// <param name="maxEndTime">The maximum end time for events.</param>
    /// <param name="minStartTime">The minimum start time for events.</param>
    /// <param name="maxStartTime">The maximum start time for events.</param>
    /// <param name="includeComments">Whether to include event comments.</param>
    /// <param name="includeHidden">Whether to include hidden events.</param>
    /// <param name="addProfileInfo">Whether to add profile information.</param>
    /// <param name="scheduled">Whether to include scheduled events.</param>
    /// <param name="order">The sort order for events.</param>
    /// <param name="max">The maximum number of events to retrieve.</param>
    /// <returns>A formatted URL string for the events endpoint.</returns>
    string GetEventsUrl(string groupId, string subGroupId, DateTime? minEndTime, DateTime? maxEndTime, DateTime? minStartTime, DateTime? maxStartTime, bool? includeComments, bool? includeHidden, bool? addProfileInfo, bool? scheduled, Enums.Order? order, int? max);
}
