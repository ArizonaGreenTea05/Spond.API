using Microsoft.Extensions.Logging;
using Spond.API.Interfaces;
using Spond.API.Models;
using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using Newtonsoft.Json;
using static Spond.API.Enums;
using JsonDocument = System.Text.Json.JsonDocument;

namespace Spond.API.Services;

/// <summary>
/// Main client for interacting with the Spond API.
/// Provides methods for authentication and retrieving data from Spond.
/// </summary>
public class SpondClient
{
    private readonly HttpClient _client;
    private readonly ICommonData _commonData;
    private readonly ILogger<SpondClient>? _logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="SpondClient"/> class.
    /// </summary>
    /// <param name="commonData">Optional common data configuration. If null, defaults to CommonData_Core_V1.</param>
    /// <param name="logger">Optional logger for logging client operations.</param>
    public SpondClient(ICommonData? commonData = null, ILogger<SpondClient>? logger = null)
    {
        _commonData = commonData ?? new CommonData_Core_V1();
        _client = new HttpClient(new HttpClientHandler { CookieContainer = new CookieContainer() }) { BaseAddress = new Uri(_commonData.BaseUrl) };
        _logger = logger;
    }

    /// <summary>
    /// Authenticates with the Spond API using an email address and password.
    /// When the account requires two-factor authentication, the <paramref name="otpCallback"/>
    /// is invoked with the masked phone number that received the one-time code.
    /// The callback must return the OTP entered by the user.
    /// If <paramref name="otpCallback"/> is <c>null</c> and 2FA is required, the login fails.
    /// </summary>
    /// <param name="email">The user's email address.</param>
    /// <param name="password">The user's password.</param>
    /// <param name="otpCallback">
    /// Optional async callback invoked when a one-time password is needed.
    /// Receives the masked phone number (e.g. "****12") and must return the OTP code.
    /// </param>
    /// <returns>True if login was successful, false otherwise.</returns>
    public async Task<bool> LoginWithEmail(string email, string password, Func<string, Task<string>>? otpCallback = null)
    {
        var loginPayload = new { email, password };
        return await Login(loginPayload, otpCallback);
    }

    /// <summary>
    /// Authenticates with the Spond API using a phone number and password.
    /// When the account requires two-factor authentication, the <paramref name="otpCallback"/>
    /// is invoked with the masked phone number that received the one-time code.
    /// The callback must return the OTP entered by the user.
    /// If <paramref name="otpCallback"/> is <c>null</c> and 2FA is required, the login fails.
    /// </summary>
    /// <param name="phoneNumber">The user's phone number.</param>
    /// <param name="password">The user's password.</param>
    /// <param name="otpCallback">
    /// Optional async callback invoked when a one-time password is needed.
    /// Receives the masked phone number (e.g. "****12") and must return the OTP code.
    /// </param>
    /// <returns>True if login was successful, false otherwise.</returns>
    public async Task<bool> LoginWithPhoneNumber(string phoneNumber, string password, Func<string, Task<string>>? otpCallback = null)
    {
        var loginPayload = new { phoneNumber, password };
        return await Login(loginPayload, otpCallback);
    }

    /// <summary>
    /// Internal method to handle the login process with different payload types.
    /// Supports the two-factor authentication flow: if the API responds with a
    /// temporary token and a masked phone number instead of a login token, the
    /// <paramref name="otpCallback"/> is used to obtain the one-time password and
    /// a second verification request is sent to complete authentication.
    /// </summary>
    /// <typeparam name="T">The type of the login payload (email or phone number).</typeparam>
    /// <param name="loginPayload">The login credentials payload.</param>
    /// <param name="otpCallback">
    /// Optional async callback invoked when 2FA is required.
    /// Receives the masked phone number and must return the OTP code.
    /// </param>
    /// <returns>True if login was successful, false otherwise.</returns>
    private async Task<bool> Login<T>(T loginPayload, Func<string, Task<string>>? otpCallback)
    {
        var loginResp = await _client.PostAsJsonAsync(_commonData.LoginUrl, loginPayload);
        if (!loginResp.IsSuccessStatusCode)
        {
            _logger?.LogError("Error logging in: {StatusCode}", loginResp.StatusCode);
            return false;
        }

        var loginJson = await loginResp.Content.ReadAsStringAsync();
        using var doc = JsonDocument.Parse(loginJson);
        var root = doc.RootElement;

        // Happy path: direct login token returned (no 2FA).
        if (root.TryGetProperty(_commonData.LoginTokenPropertyName, out var tokenElement))
        {
            var loginToken = tokenElement.GetString();
            _client.DefaultRequestHeaders.Authorization =
                new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", loginToken);
            return true;
        }

        // 2FA path: API returned a temporary token and the masked destination phone number.
        if (root.TryGetProperty("token", out var tempTokenElement) &&
            root.TryGetProperty("phoneNumber", out var phoneElement))
        {
            var tempToken = tempTokenElement.GetString();
            var maskedPhone = phoneElement.GetString() ?? string.Empty;

            if (otpCallback is null)
            {
                _logger?.LogError(
                    "Login requires a one-time password sent to {Phone}. " +
                    "Provide an otpCallback to handle two-factor authentication.",
                    maskedPhone);
                return false;
            }

            var otpCode = await otpCallback(maskedPhone);
            if (string.IsNullOrWhiteSpace(otpCode))
            {
                _logger?.LogError("OTP callback returned an empty code.");
                return false;
            }

            var otpPayload = new { code = otpCode, token = tempToken };
            var otpResp = await _client.PostAsJsonAsync(_commonData.LoginUrl, otpPayload);
            if (!otpResp.IsSuccessStatusCode)
            {
                _logger?.LogError("OTP verification failed: {StatusCode}", otpResp.StatusCode);
                return false;
            }

            var otpJson = await otpResp.Content.ReadAsStringAsync();
            using var otpDoc = JsonDocument.Parse(otpJson);
            if (!otpDoc.RootElement.TryGetProperty(_commonData.LoginTokenPropertyName, out var finalTokenElement))
            {
                _logger?.LogError("OTP verification response did not contain a login token.");
                return false;
            }

            var finalToken = finalTokenElement.GetString();
            _client.DefaultRequestHeaders.Authorization =
                new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", finalToken);
            return true;
        }

        _logger?.LogError("Unexpected login response: {Response}", loginJson);
        return false;
    }

    /// <summary>
    /// Generic method to retrieve data from the Spond API.
    /// </summary>
    /// <typeparam name="T">The type to deserialize the response data into.</typeparam>
    /// <param name="url">The API endpoint URL.</param>
    /// <returns>The deserialized data object, or null if the request failed.</returns>
    public async Task<T?> GetData<T>(string url) where T : class
    {
        var response = await _client.GetAsync(url);
        if (!response.IsSuccessStatusCode) return null;
        var json = await response.Content.ReadAsStringAsync();
        return JsonConvert.DeserializeObject<T>(json);
    }

    /// <summary>
    /// Retrieves all groups the authenticated user belongs to.
    /// </summary>
    /// <returns>A list of <see cref="SpondGroup"/> objects, or an empty list if none found.</returns>
    public async Task<List<SpondGroup>> GetGroups() => await GetData<List<SpondGroup>>(_commonData.GroupsUrl) ?? [];

    /// <summary>
    /// Retrieves the profile information of the currently authenticated user.
    /// </summary>
    /// <returns>The <see cref="SpondCompleteUserProfile"/> of the current user, or null if not found.</returns>
    public async Task<SpondCompleteUserProfile?> GetCurrentUser() => await GetData<SpondCompleteUserProfile>(_commonData.UserUrl);

    /// <summary>
    /// Retrieves events for all groups within a specified time range.
    /// </summary>
    /// <param name="minEndTime">The minimum end time for events to retrieve.</param>
    /// <param name="maxEndTime">The maximum end time for events to retrieve.</param>
    /// <param name="max">Optional maximum number of events to retrieve.</param>
    /// <param name="order">The order to sort events (Ascending or Descending).</param>
    /// <param name="scheduled">Include scheduled events.</param>
    /// <param name="includeHidden">Include hidden events.</param>
    /// <param name="includeComments">Include event comments.</param>
    /// <param name="addProfileInfo">Add profile information to the events.</param>
    /// <returns>A list of <see cref="SpondEvent"/> objects, or an empty list if none found.</returns>
    public async Task<List<SpondEvent>> GetEvents(DateTime minEndTime, DateTime maxEndTime, int? max = null,
        Order order = Order.Ascending, bool scheduled = true, bool includeHidden = false, bool includeComments = true,
        bool addProfileInfo = true)
    {
        return await GetData<List<SpondEvent>>(_commonData.GetEventsUrl(minEndTime, maxEndTime, includeComments, includeHidden, addProfileInfo, scheduled, order, max)) ?? [];
    }

    /// <summary>
    /// Retrieves events for a specific group within a specified time range.
    /// </summary>
    /// <param name="group">The group to retrieve events for.</param>
    /// <param name="minEndTime">The minimum end time for events to retrieve.</param>
    /// <param name="maxEndTime">The maximum end time for events to retrieve.</param>
    /// <param name="max">Optional maximum number of events to retrieve.</param>
    /// <param name="order">The order to sort events (Ascending or Descending).</param>
    /// <param name="scheduled">Include scheduled events.</param>
    /// <param name="includeHidden">Include hidden events.</param>
    /// <param name="includeComments">Include event comments.</param>
    /// <param name="addProfileInfo">Add profile information to the events.</param>
    /// <returns>A list of <see cref="SpondEvent"/> objects, or an empty list if none found.</returns>
    public async Task<List<SpondEvent>> GetEvents(SpondGroup group, DateTime minEndTime, DateTime maxEndTime,
        int? max = null, Order order = Order.Ascending, bool scheduled = true, bool includeHidden = false,
        bool includeComments = true, bool addProfileInfo = true) 
        => await GetEvents(group.Id, minEndTime, maxEndTime, max, order, scheduled, includeHidden, includeComments, addProfileInfo);

    /// <summary>
    /// Retrieves events for a specific group (by ID) within a specified time range.
    /// </summary>
    /// <param name="groupId">The ID of the group to retrieve events for.</param>
    /// <param name="minEndTime">The minimum end time for events to retrieve.</param>
    /// <param name="maxEndTime">The maximum end time for events to retrieve.</param>
    /// <param name="max">Optional maximum number of events to retrieve.</param>
    /// <param name="order">The order to sort events (Ascending or Descending).</param>
    /// <param name="scheduled">Include scheduled events.</param>
    /// <param name="includeHidden">Include hidden events.</param>
    /// <param name="includeComments">Include event comments.</param>
    /// <param name="addProfileInfo">Add profile information to the events.</param>
    /// <returns>A list of <see cref="SpondEvent"/> objects, or an empty list if none found.</returns>
    public async Task<List<SpondEvent>> GetEvents(string groupId, DateTime minEndTime, DateTime maxEndTime,
        int? max = null, Order order = Order.Ascending, bool scheduled = true, bool includeHidden = false,
        bool includeComments = true, bool addProfileInfo = true)
    {
        return await GetData<List<SpondEvent>>(_commonData.GetEventsUrl(groupId, minEndTime, maxEndTime, includeComments, includeHidden, addProfileInfo, scheduled, order, max)) ?? [];
    }

    /// <summary>
    /// Retrieves events for a specific subgroup within a group and time range.
    /// </summary>
    /// <param name="group">The parent group.</param>
    /// <param name="subGroup">The subgroup to retrieve events for.</param>
    /// <param name="minEndTime">The minimum end time for events to retrieve.</param>
    /// <param name="maxEndTime">The maximum end time for events to retrieve.</param>
    /// <param name="max">Optional maximum number of events to retrieve.</param>
    /// <param name="order">The order to sort events (Ascending or Descending).</param>
    /// <param name="scheduled">Include scheduled events.</param>
    /// <param name="includeHidden">Include hidden events.</param>
    /// <param name="includeComments">Include event comments.</param>
    /// <param name="addProfileInfo">Add profile information to the events.</param>
    /// <returns>A list of <see cref="SpondEvent"/> objects, or an empty list if none found.</returns>
    public async Task<List<SpondEvent>> GetEvents(SpondGroup group, SpondSubGroup subGroup, DateTime minEndTime, DateTime maxEndTime,
        int? max = null, Order order = Order.Ascending, bool scheduled = true, bool includeHidden = false,
        bool includeComments = true, bool addProfileInfo = true)
        => await GetEvents(group.Id, subGroup.Id, minEndTime, maxEndTime, max, order, scheduled, includeHidden, includeComments, addProfileInfo);

    /// <summary>
    /// Retrieves events for a specific subgroup (by IDs) within a specified time range.
    /// </summary>
    /// <param name="groupId">The ID of the parent group.</param>
    /// <param name="subGroupId">The ID of the subgroup to retrieve events for.</param>
    /// <param name="minEndTime">The minimum end time for events to retrieve.</param>
    /// <param name="maxEndTime">The maximum end time for events to retrieve.</param>
    /// <param name="max">Optional maximum number of events to retrieve.</param>
    /// <param name="order">The order to sort events (Ascending or Descending).</param>
    /// <param name="scheduled">Include scheduled events.</param>
    /// <param name="includeHidden">Include hidden events.</param>
    /// <param name="includeComments">Include event comments.</param>
    /// <param name="addProfileInfo">Add profile information to the events.</param>
    /// <returns>A list of <see cref="SpondEvent"/> objects, or an empty list if none found.</returns>
    public async Task<List<SpondEvent>> GetEvents(string groupId, string subGroupId, DateTime minEndTime, DateTime maxEndTime, int? max = null,
        Order order = Order.Ascending, bool scheduled = true, bool includeHidden = false, bool includeComments = true,
        bool addProfileInfo = true)
    {
        return await GetData<List<SpondEvent>>(_commonData.GetEventsUrl(groupId, subGroupId, minEndTime, maxEndTime, includeComments, includeHidden, addProfileInfo, scheduled, order, max)) ?? [];
    }
}
