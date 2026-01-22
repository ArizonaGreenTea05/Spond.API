namespace Spond.API.Interfaces;

public interface ICommonData
{
    string LoginTokenPropertyName { get; }
    string BaseUrl { get; }
    string LoginUrl { get; }
    string UserUrl { get; }
    string GroupsUrl { get; }

    string GetEventsUrl(DateTime minEndTime, DateTime maxEndTime, bool? includeComments, bool? includeHidden, bool? addProfileInfo, bool? scheduled, Enums.Order? order, int? max);

    string GetEventsUrl(string groupId, DateTime minEndTime, DateTime maxEndTime, bool? includeComments, bool? includeHidden, bool? addProfileInfo, bool? scheduled, Enums.Order? order, int? max);

    string GetEventsUrl(string groupId, string subGroupId, DateTime minEndTime, DateTime maxEndTime, bool? includeComments, bool? includeHidden, bool? addProfileInfo, bool? scheduled, Enums.Order? order, int? max);
}
