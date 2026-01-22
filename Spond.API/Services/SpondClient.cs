using Microsoft.Extensions.Logging;
using Spond.API.Extensions;
using Spond.API.Interfaces;
using Spond.API.Models;
using System.Net;
using System.Net.Http.Json;
using Newtonsoft.Json;
using static Spond.API.Enums;
using JsonDocument = System.Text.Json.JsonDocument;

namespace Spond.API.Services;

public class SpondClient
{
    private readonly HttpClient _client;
    private readonly ICommonData _commonData;
    private readonly ILogger<SpondClient>? _logger;

    public SpondClient(ICommonData? commonData = null, ILogger<SpondClient>? logger = null)
    {
        _commonData = commonData ?? new CommonData_2_1();
        _client = new HttpClient(new HttpClientHandler { CookieContainer = new CookieContainer() }) { BaseAddress = new Uri(_commonData.BaseUrl) };
        _logger = logger;
    }

    public async Task<bool> LoginWithEmail(string email, string password)
    {
        var loginPayload = new { email, password };
        return await Login(loginPayload);
    }

    public async Task<bool> LoginWithPhoneNumber(string phoneNumber, string password)
    {
        var loginPayload = new { phoneNumber, password };
        return await Login(loginPayload);
    }

    private async Task<bool> Login<T>(T loginPayload)
    {
        var loginResp = await _client.PostAsJsonAsync(_commonData.LoginUrl, loginPayload);
        if (!loginResp.IsSuccessStatusCode)
        {
            _logger?.LogError($"Error logging in: {loginResp.StatusCode} - {loginResp.Content.ReadAsStringAsync()}");
            return false;
        }
        var loginJson = await loginResp.Content.ReadAsStringAsync();
        using var doc = JsonDocument.Parse(loginJson);
        var loginToken = doc.RootElement.GetProperty(_commonData.LoginTokenPropertyName).GetString();
        _client.DefaultRequestHeaders.Authorization =
            new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", loginToken);
        return true;
    }

    public async Task<T?> GetData<T>(string url) where T : class
    {
        var response = await _client.GetAsync(url);
        if (!response.IsSuccessStatusCode) return null;
        var json = await response.Content.ReadAsStringAsync();
        return JsonConvert.DeserializeObject<T>(json);
    }

    public async Task<List<SpondGroup>> GetGroups() => await GetData<List<SpondGroup>>(_commonData.GroupsUrl) ?? [];

    public async Task<SpondUserProfile?> GetCurrentUser() => await GetData<SpondUserProfile>(_commonData.UserUrl);

    public async Task<List<SpondEvent>> GetEvents(DateTime minEndTime, DateTime maxEndTime, int? max = null,
        Order order = Order.Ascending, bool scheduled = true, bool includeHidden = false, bool includeComments = true,
        bool addProfileInfo = true)
    {
        return await GetData<List<SpondEvent>>(_commonData.GetEventsUrl(minEndTime, maxEndTime, includeComments, includeHidden, addProfileInfo, scheduled, order, max)) ?? [];
    }

    public async Task<List<SpondEvent>> GetEvents(SpondGroup group, DateTime minEndTime, DateTime maxEndTime,
        int? max = null, Order order = Order.Ascending, bool scheduled = true, bool includeHidden = false,
        bool includeComments = true, bool addProfileInfo = true) 
        => await GetEvents(group.Id, minEndTime, maxEndTime, max, order, scheduled, includeHidden, includeComments, addProfileInfo);

    public async Task<List<SpondEvent>> GetEvents(string groupId, DateTime minEndTime, DateTime maxEndTime,
        int? max = null, Order order = Order.Ascending, bool scheduled = true, bool includeHidden = false,
        bool includeComments = true, bool addProfileInfo = true)
    {
        return await GetData<List<SpondEvent>>(_commonData.GetEventsUrl(groupId, minEndTime, maxEndTime, includeComments, includeHidden, addProfileInfo, scheduled, order, max)) ?? [];
    }

    public async Task<List<SpondEvent>> GetEvents(SpondGroup group, SpondSubGroup subGroup, DateTime minEndTime, DateTime maxEndTime,
        int? max = null, Order order = Order.Ascending, bool scheduled = true, bool includeHidden = false,
        bool includeComments = true, bool addProfileInfo = true)
        => await GetEvents(group.Id, subGroup.Id, minEndTime, maxEndTime, max, order, scheduled, includeHidden, includeComments, addProfileInfo);

    public async Task<List<SpondEvent>> GetEvents(string groupId, string subGroupId, DateTime minEndTime, DateTime maxEndTime, int? max = null,
        Order order = Order.Ascending, bool scheduled = true, bool includeHidden = false, bool includeComments = true,
        bool addProfileInfo = true)
    {
        return await GetData<List<SpondEvent>>(_commonData.GetEventsUrl(groupId, subGroupId, minEndTime, maxEndTime, includeComments, includeHidden, addProfileInfo, scheduled, order, max)) ?? [];
    }
}
