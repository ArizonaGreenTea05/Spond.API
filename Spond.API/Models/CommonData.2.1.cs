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
    public string NestedLoginTokenPropertyName => "token";
    /// <inheritdoc/>
    public string BaseUrl => "https://spond.com/";
    /// <inheritdoc/>
    public string LoginUrl => "/api/2.1/login";
    /// <inheritdoc/>
    public string UserUrl => "/api/2.1/profile";
    /// <inheritdoc/>
    public string GroupsUrl => "/api/2.1/groups";
    /// <inheritdoc/>
    public string ChatUrl => "/api/2.1/chat";

    /// <inheritdoc/>
    public string GetPostsUrl(int max, bool includeComments, string? groupId = null)
    {
        var parameters = new List<string>
        {
            "type=PLAIN",
            $"max={max}",
            $"includeComments={includeComments.ToString().ToLower()}"
        };
        if (groupId is not null) parameters.Add($"groupId={groupId}");
        return $"/api/2.1/posts/?{string.Join('&', parameters)}";
    }

    /// <inheritdoc/>
    public string GetEventUrl(string eventId) => $"/api/2.1/sponds/{eventId}";

    /// <inheritdoc/>
    public string GetEventAttendanceUrl(string eventId) => $"/api/2.1/sponds/{eventId}/export";

    /// <inheritdoc/>
    public string GetEventResponseUrl(string eventId, string userId) => $"/api/2.1/sponds/{eventId}/responses/{userId}";

    /// <summary>
    /// Constructs the events URL with query parameters.
    /// </summary>
    private static string GetEventsUrl(List<string> parameters)
    {
        return $"/api/2.1/sponds{(parameters.Count > 0 ? "?" : string.Empty)}{string.Join('&', parameters)}";
    }

    /// <summary>
    /// Builds the list of query parameters for event requests.
    /// </summary>
    private static List<string> GetEventsParameters(DateTime? minEndTime, DateTime? maxEndTime, DateTime? minStartTime, DateTime? maxStartTime,
        bool? includeComments, bool? includeHidden, bool? addProfileInfo, bool? scheduled, Enums.Order? order, int? max, string? groupId, string? subGroupId)
    {
        var parameters = new List<string>
        {
            $"max={max ?? 100}",
            $"order={order switch
            {
                Enums.Order.Ascending => "asc",
                Enums.Order.Descending => "desc",
                _ => "asc"
            }}"
        };

        if (minEndTime is not null) parameters.Add($"minEndTimestamp={minEndTime.Value.ToIso8601(true)}");
        if (maxEndTime is not null) parameters.Add($"maxEndTimestamp={maxEndTime.Value.ToIso8601(true)}");
        if (minStartTime is not null) parameters.Add($"minStartTimestamp={minStartTime.Value.ToIso8601(true)}");
        if (maxStartTime is not null) parameters.Add($"maxStartTimestamp={maxStartTime.Value.ToIso8601(true)}");
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
        return GetEventsUrl(GetEventsParameters(minEndTime, maxEndTime, null, null, includeComments, includeHidden, addProfileInfo, scheduled, order, max, null, null));
    }

    /// <inheritdoc/>
    public string GetEventsUrl(string groupId, DateTime minEndTime, DateTime maxEndTime, bool? includeComments, bool? includeHidden, bool? addProfileInfo, bool? scheduled, Enums.Order? order, int? max)
    {
        return GetEventsUrl(GetEventsParameters(minEndTime, maxEndTime, null, null, includeComments, includeHidden, addProfileInfo, scheduled, order, max, groupId, null));
    }

    /// <inheritdoc/>
    public string GetEventsUrl(string groupId, string subGroupId, DateTime minEndTime, DateTime maxEndTime, bool? includeComments, bool? includeHidden, bool? addProfileInfo, bool? scheduled, Enums.Order? order, int? max)
    {
        return GetEventsUrl(GetEventsParameters(minEndTime, maxEndTime, null, null, includeComments, includeHidden, addProfileInfo, scheduled, order, max, groupId, subGroupId));
    }

    /// <inheritdoc/>
    public string GetEventsUrl(DateTime? minEndTime, DateTime? maxEndTime, DateTime? minStartTime, DateTime? maxStartTime, bool? includeComments, bool? includeHidden, bool? addProfileInfo, bool? scheduled, Enums.Order? order, int? max)
    {
        return GetEventsUrl(GetEventsParameters(minEndTime, maxEndTime, minStartTime, maxStartTime, includeComments, includeHidden, addProfileInfo, scheduled, order, max, null, null));
    }

    /// <inheritdoc/>
    public string GetEventsUrl(string groupId, DateTime? minEndTime, DateTime? maxEndTime, DateTime? minStartTime, DateTime? maxStartTime, bool? includeComments, bool? includeHidden, bool? addProfileInfo, bool? scheduled, Enums.Order? order, int? max)
    {
        return GetEventsUrl(GetEventsParameters(minEndTime, maxEndTime, minStartTime, maxStartTime, includeComments, includeHidden, addProfileInfo, scheduled, order, max, groupId, null));
    }

    /// <inheritdoc/>
    public string GetEventsUrl(string groupId, string subGroupId, DateTime? minEndTime, DateTime? maxEndTime, DateTime? minStartTime, DateTime? maxStartTime, bool? includeComments, bool? includeHidden, bool? addProfileInfo, bool? scheduled, Enums.Order? order, int? max)
    {
        return GetEventsUrl(GetEventsParameters(minEndTime, maxEndTime, minStartTime, maxStartTime, includeComments, includeHidden, addProfileInfo, scheduled, order, max, groupId, subGroupId));
    }
}

