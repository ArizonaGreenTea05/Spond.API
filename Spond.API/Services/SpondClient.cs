using Microsoft.Extensions.Logging;
using Spond.API.Interfaces;
using Spond.API.Models;
using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using Spond.API.Extensions;

namespace Spond.API.Services;

public class SpondClient
{
    private readonly HttpClient _client;
    private readonly ICommonData _commonData;
    private readonly ILogger<SpondClient>? _logger;

    public SpondClient(ICommonData? commonData = null, ILogger<SpondClient>? logger = null)
    {
        _commonData = commonData ?? new CommonData_2_1();
        _client = new(new HttpClientHandler { CookieContainer = new CookieContainer() }) { BaseAddress = new Uri(_commonData.BaseUrl) };
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

    public async Task<List<SpondGroup>> GetGroups()
    {
        var groupsResp = await _client.GetAsync(_commonData.GroupsUrl);
        if (!groupsResp.IsSuccessStatusCode) return [];
        var groupsJson = await groupsResp.Content.ReadAsStringAsync();
        return JsonSerializer.Deserialize<List<SpondGroup>>(groupsJson, new JsonSerializerOptions { PropertyNameCaseInsensitive = true }) ?? [];
    }

    public async Task<SpondUserProfile?> GetCurrentUser()
    {
        var userResp = await _client.GetAsync(_commonData.UserUrl);
        if (!userResp.IsSuccessStatusCode) return null;
        var userJson = await userResp.Content.ReadAsStringAsync();
        return JsonSerializer.Deserialize<SpondUserProfile>(userJson, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
    }

    public async Task<List<SpondEvent>> GetEvents(DateTime minEndTime, DateTime maxEndTime, int max = 200, Enums.Order order = Enums.Order.Ascending, bool scheduled = true, bool includeHidden = false, bool includeComments = true, bool addProfileInfo = true)
    {
        var url = string.Format(_commonData.EventsUrl, includeComments.ToString().ToLower(),
            includeHidden.ToString().ToLower(), addProfileInfo.ToString().ToLower(), scheduled.ToString().ToLower(),
            order switch
            {
                Enums.Order.Ascending => "asc",
                Enums.Order.Descending => "desc",
                _ => "asc"
            },
            max, minEndTime.ToIso8601(true), maxEndTime.ToIso8601(true));
        var eventsResp = await _client.GetAsync(url);
        if (!eventsResp.IsSuccessStatusCode) return [];
        var eventsJson = await eventsResp.Content.ReadAsStringAsync();
        return JsonSerializer.Deserialize<List<SpondEvent>>(eventsJson, new JsonSerializerOptions { PropertyNameCaseInsensitive = true }) ?? [];
    }
}
