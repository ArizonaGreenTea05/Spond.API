using Spond.API.Extensions;
using Spond.API.Interfaces;

namespace Spond.API.Models;

/// <summary>
/// Implementation of ICommonData for Spond API version 2.1.
/// Provides API endpoints and URL construction for version 2.1 of the Spond API.
/// </summary>
internal class CommonData_2_1 : ICommonData
{
    /// <inheritdoc/>
    public string LoginTokenPropertyName => "loginToken";
    /// <inheritdoc/>
    public string BaseUrl => "https://spond.com/";
    /// <inheritdoc/>
    public string LoginUrl => "/api/2.1/login";
    /// <inheritdoc/>
    public string UserUrl => "/api/2.1/profile";
    /// <inheritdoc/>
    public string GroupsUrl => "/api/2.1/groups";

    /// <summary>
    /// Constructs the events URL with query parameters.
    /// </summary>
    /// <param name="parameters">List of query parameters.</param>
    /// <returns>The complete events URL with query string.</returns>
    private static string GetEventsUrl(List<string> parameters)
    {
        return $"/api/2.1/sponds{(parameters.Count > 0 ? "?" : string.Empty)}{string.Join('&', parameters)}";
    }

    /// <summary>
    /// Builds the list of query parameters for event requests.
    /// </summary>
    /// <param name="minEndTime">The minimum end time for events.</param>
    /// <param name="maxEndTime">The maximum end time for events.</param>
    /// <param name="includeComments">Whether to include event comments.</param>
    /// <param name="includeHidden">Whether to include hidden events.</param>
    /// <param name="addProfileInfo">Whether to add profile information.</param>
    /// <param name="scheduled">Whether to include scheduled events.</param>
    /// <param name="order">The sort order for events.</param>
    /// <param name="max">The maximum number of events to retrieve.</param>
    /// <param name="groupId">Optional group ID filter.</param>
    /// <param name="subGroupId">Optional subgroup ID filter.</param>
    /// <returns>A list of formatted query parameter strings.</returns>
    private static List<string> GetEventsParameters(DateTime minEndTime, DateTime maxEndTime, bool? includeComments, bool? includeHidden, bool? addProfileInfo, bool? scheduled, Enums.Order? order, int? max, string? groupId, string? subGroupId)
    {
        var parameters = new List<string>
        {
            $"minEndTimestamp={minEndTime.ToIso8601(true)}",
            $"maxEndTimestamp={maxEndTime.ToIso8601(true)}",
            $"max={max ?? (int)Math.Max(1, Math.Round((maxEndTime - minEndTime).TotalDays * 5))}",
            $"order={order switch
            {
                Enums.Order.Ascending => "asc",
                Enums.Order.Descending => "desc",
                _ => "asc"
            }}"
        };

        if (includeComments is not null) parameters.Add($"includeComments={includeComments.ToString()?.ToLower()}");
        if (includeHidden is not null) parameters.Add($"includeHidden={includeHidden.ToString()?.ToLower()}");
        if (addProfileInfo is not null) parameters.Add($"addProfileInfo={addProfileInfo.ToString()?.ToLower()}");
        if (scheduled is not null) parameters.Add($"scheduled={scheduled.ToString()?.ToLower()}");
        if (groupId is not null) parameters.Add($"groupId={groupId}");
        if (subGroupId is not null) parameters.Add($"subGroupId={subGroupId}");

        return parameters;
    }

    /// <inheritdoc/>
    public string GetEventsUrl(DateTime minEndTime, DateTime maxEndTime, bool? includeComments, bool? includeHidden, bool? addProfileInfo, bool? scheduled, Enums.Order? order, int? max)
    {
        return GetEventsUrl(GetEventsParameters(minEndTime, maxEndTime, includeComments, includeHidden, addProfileInfo, scheduled, order, max, null, null));
    }

    /// <inheritdoc/>
    public string GetEventsUrl(string groupId, DateTime minEndTime, DateTime maxEndTime, bool? includeComments, bool? includeHidden, bool? addProfileInfo, bool? scheduled, Enums.Order? order, int? max)
    {
        return GetEventsUrl(GetEventsParameters(minEndTime, maxEndTime, includeComments, includeHidden, addProfileInfo, scheduled, order, max, groupId, null));
    }

    /// <inheritdoc/>
    public string GetEventsUrl(string groupId, string subGroupId, DateTime minEndTime, DateTime maxEndTime, bool? includeComments, bool? includeHidden, bool? addProfileInfo, bool? scheduled, Enums.Order? order, int? max)
    {
        return GetEventsUrl(GetEventsParameters(minEndTime, maxEndTime, includeComments, includeHidden, addProfileInfo, scheduled, order, max, groupId, subGroupId));
    }
}
