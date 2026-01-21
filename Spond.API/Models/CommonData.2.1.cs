using Spond.API.Interfaces;

namespace Spond.API.Models;

internal class CommonData_2_1 : ICommonData
{
    public string LoginTokenPropertyName => "loginToken";
    public string BaseUrl => "https://spond.com/";
    public string LoginUrl => "/api/2.1/login";
    public string UserUrl => "/api/2.1/profile";
    public string GroupsUrl => "/api/2.1/groups";

    /// <summary>
    ///
    /// <param name="includeComments">true or false</param>
    /// <param name="includeHidden">true or false</param>
    /// <param name="addProfileInfo">true or false</param>
    /// <param name="scheduled">true or false</param>
    /// <param name="order">asc or desc</param>
    /// <param name="max">integer</param>
    /// <param name="minEndTimestamp">2025-09-29T22:00:00.001Z</param>
    /// <param name="maxEndTimestamp">2025-09-29T22:00:00.001Z</param>
    /// </summary>
    public string EventsUrl => "/api/2.1/sponds?includeComments={0}&includeHidden={1}&addProfileInfo={2}&scheduled={3}&order={4}&max={5}&minEndTimestamp={6}&maxEndTimestamp={7}";
}
