using Spond.API.Extensions;
using Spond.API.Interfaces;

namespace Spond.API.Models;

public class CommonData_2_1 : ICommonData
{
    public string LoginTokenPropertyName => "loginToken";
    public string BaseUrl => "https://spond.com/";
    public string LoginUrl => "/api/2.1/login";
    public string UserUrl => "/api/2.1/profile";
    public string GroupsUrl => "/api/2.1/groups";

    private static string GetEventsUrl(List<string> parameters)
    {
        return $"/api/2.1/sponds{(parameters.Count > 0 ? "?" : string.Empty)}{string.Join('&', parameters)}";
    }

    private static List<string> GetEventsParameters(DateTime minEndTime, DateTime maxEndTime, bool? includeComments, bool? includeHidden, bool? addProfileInfo, bool? scheduled, Enums.Order? order, int? max, string? groupId, string? subGroupId)
    {
        var parameters = new List<string>
        {
            $"minEndTimestamp={minEndTime.ToIso8601(true)}",
            $"maxEndTimestamp={maxEndTime.ToIso8601(true)}",
            $"max={max ?? 2000}",
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

    public string GetEventsUrl(DateTime minEndTime, DateTime maxEndTime, bool? includeComments, bool? includeHidden, bool? addProfileInfo, bool? scheduled, Enums.Order? order, int? max)
    {
        return GetEventsUrl(GetEventsParameters(minEndTime, maxEndTime, includeComments, includeHidden, addProfileInfo, scheduled, order, max, null, null));
    }

    public string GetEventsUrl(string groupId, DateTime minEndTime, DateTime maxEndTime, bool? includeComments, bool? includeHidden, bool? addProfileInfo, bool? scheduled, Enums.Order? order, int? max)
    {
        return GetEventsUrl(GetEventsParameters(minEndTime, maxEndTime, includeComments, includeHidden, addProfileInfo, scheduled, order, max, groupId, null));
    }

    public string GetEventsUrl(string groupId, string subGroupId, DateTime minEndTime, DateTime maxEndTime, bool? includeComments, bool? includeHidden, bool? addProfileInfo, bool? scheduled, Enums.Order? order, int? max)
    {
        return GetEventsUrl(GetEventsParameters(minEndTime, maxEndTime, includeComments, includeHidden, addProfileInfo, scheduled, order, max, groupId, subGroupId));
    }
}
