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
}
