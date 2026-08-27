using Spond.API.Extensions;
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

    private string? _chatServerUrl;
    private string? _chatAuth;
    private HttpClient? _chatClient;

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
            if(tokenElement.ValueKind == JsonValueKind.Object)
            {
                tokenElement.TryGetProperty(_commonData.NestedLoginTokenPropertyName, out tokenElement);
            }
            var loginToken = tokenElement.GetString();
            if (string.IsNullOrEmpty(loginToken))
            {
                _logger?.LogError("Login response contained an empty login token.");
                return false;
            }
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
            if (string.IsNullOrEmpty(finalToken))
            {
                _logger?.LogError("OTP verification response contained an empty login token.");
                return false;
            }
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
    /// Retrieves events for all groups with flexible start and end time filters.
    /// </summary>
    /// <param name="minEndTime">Optional minimum end time for events.</param>
    /// <param name="maxEndTime">Optional maximum end time for events.</param>
    /// <param name="minStartTime">Optional minimum start time for events.</param>
    /// <param name="maxStartTime">Optional maximum start time for events.</param>
    /// <param name="max">Optional maximum number of events to retrieve.</param>
    /// <param name="order">The order to sort events (Ascending or Descending).</param>
    /// <param name="scheduled">Include scheduled events.</param>
    /// <param name="includeHidden">Include hidden events.</param>
    /// <param name="includeComments">Include event comments.</param>
    /// <param name="addProfileInfo">Add profile information to the events.</param>
    /// <returns>A list of <see cref="SpondEvent"/> objects, or an empty list if none found.</returns>
    public async Task<List<SpondEvent>> GetEvents(DateTime? minEndTime = null, DateTime? maxEndTime = null,
        DateTime? minStartTime = null, DateTime? maxStartTime = null, int? max = null,
        Order order = Order.Ascending, bool scheduled = true, bool includeHidden = false,
        bool includeComments = true, bool addProfileInfo = true)
    {
        return await GetData<List<SpondEvent>>(_commonData.GetEventsUrl(minEndTime, maxEndTime, minStartTime, maxStartTime, includeComments, includeHidden, addProfileInfo, scheduled, order, max)) ?? [];
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

    /// <summary>
    /// Retrieves a single event by its unique identifier.
    /// </summary>
    /// <param name="eventId">The unique identifier of the event.</param>
    /// <returns>The <see cref="SpondEvent"/>, or null if not found.</returns>
    public async Task<SpondEvent?> GetEvent(string eventId) => await GetData<SpondEvent>(_commonData.GetEventUrl(eventId));

    /// <summary>
    /// Updates an existing event by merging the provided changes into the current event.
    /// </summary>
    /// <param name="eventId">The unique identifier of the event to update.</param>
    /// <param name="updates">The fields to update. Only populated properties are applied.</param>
    /// <returns>The updated <see cref="SpondEvent"/> as persisted server-side, or null if the request failed.</returns>
    public async Task<SpondEvent?> UpdateEvent(string eventId, SpondEventUpdateRequest updates)
    {
        var currentEvent = await GetEvent(eventId);
        if (currentEvent is null)
        {
            _logger?.LogError("Event with ID {EventId} not found.", eventId);
            return null;
        }

        var payload = new
        {
            id = eventId,
            heading = updates.Heading ?? currentEvent.Heading,
            description = updates.Description ?? currentEvent.Description,
            spondType = (currentEvent.SpondType ?? SpondType.Event).ToEnumMemberValue(),
            startTimestamp = updates.StartTimestamp ?? currentEvent.StartTimestamp,
            endTimestamp = updates.EndTimestamp ?? currentEvent.EndTimestamp,
            commentsDisabled = updates.CommentsDisabled ?? currentEvent.CommentsDisabled ?? false,
            maxAccepted = updates.MaxAccepted ?? currentEvent.MaxAccepted ?? 0,
            rsvpDate = updates.RsvpDate ?? currentEvent.RsvpDate,
            location = updates.Location ?? currentEvent.Location,
            owners = currentEvent.Owners.Select(o => new { id = o.Id }).ToList(),
            visibility = (updates.Visibility ?? currentEvent.Visibility ?? EventVisibility.Invitees).ToEnumMemberValue(),
            participantsHidden = updates.ParticipantsHidden ?? currentEvent.ParticipantsHidden ?? false,
            autoReminderType = (updates.AutoReminderType ?? currentEvent.AutoReminderType ?? AutoReminderType.Disabled).ToEnumMemberValue(),
            autoAccept = updates.AutoAccept ?? currentEvent.AutoAccept ?? false,
            payment = new { },
            attachments = Array.Empty<object>(),
            tasks = currentEvent.Tasks ?? new SpondEventTasks()
        };

        var url = _commonData.GetEventUrl(eventId);
        var response = await _client.PostAsJsonAsync(url, payload);
        if (!response.IsSuccessStatusCode)
        {
            _logger?.LogError("Failed to update event {EventId}: {StatusCode}", eventId, response.StatusCode);
            return null;
        }
        var json = await response.Content.ReadAsStringAsync();
        return JsonConvert.DeserializeObject<SpondEvent>(json);
    }

    /// <summary>
    /// Downloads the attendance report for an event as raw XLSX bytes.
    /// </summary>
    /// <param name="eventId">The unique identifier of the event.</param>
    /// <returns>The raw XLSX file bytes, or null if the request failed.</returns>
    public async Task<byte[]?> GetEventAttendance(string eventId)
    {
        var response = await _client.GetAsync(_commonData.GetEventAttendanceUrl(eventId));
        if (!response.IsSuccessStatusCode)
        {
            _logger?.LogError("Failed to download attendance for event {EventId}: {StatusCode}", eventId, response.StatusCode);
            return null;
        }
        return await response.Content.ReadAsByteArrayAsync();
    }

    /// <summary>
    /// Changes a member's response (accept or decline) for a specific event.
    /// </summary>
    /// <param name="eventId">The unique identifier of the event.</param>
    /// <param name="memberId">The member's ID (as found in group members, not the profile ID).</param>
    /// <param name="accepted">True to accept the invitation, false to decline.</param>
    /// <param name="declineMessage">Optional message to include when declining.</param>
    /// <returns>The updated <see cref="SpondEventResponses"/>, or null if the request failed.</returns>
    public async Task<SpondEventResponses?> ChangeResponse(string eventId, string memberId, bool accepted, string? declineMessage = null)
    {
        var payload = accepted
            ? new Dictionary<string, string> { ["accepted"] = "true" }
            : declineMessage is not null
                ? new Dictionary<string, string> { ["accepted"] = "false", ["declineMessage"] = declineMessage }
                : new Dictionary<string, string> { ["accepted"] = "false" };

        var url = _commonData.GetEventResponseUrl(eventId, memberId);
        var response = await _client.PutAsJsonAsync(url, payload);
        if (!response.IsSuccessStatusCode)
        {
            _logger?.LogError("Failed to change response for event {EventId}, member {MemberId}: {StatusCode}", eventId, memberId, response.StatusCode);
            return null;
        }
        var json = await response.Content.ReadAsStringAsync();
        return JsonConvert.DeserializeObject<SpondEventResponses>(json);
    }

    /// <summary>
    /// Retrieves posts from group walls.
    /// </summary>
    /// <param name="groupId">Optional group ID to filter posts by group.</param>
    /// <param name="max">Maximum number of posts to retrieve. Defaults to 20.</param>
    /// <param name="includeComments">Whether to include comments on posts. Defaults to true.</param>
    /// <returns>A list of <see cref="SpondPost"/> objects, or an empty list if none found.</returns>
    public async Task<List<SpondPost>> GetPosts(string? groupId = null, int max = 20, bool includeComments = true)
    {
        return await GetData<List<SpondPost>>(_commonData.GetPostsUrl(max, includeComments, groupId)) ?? [];
    }

    /// <summary>
    /// Performs the secondary authentication handshake with the Spond chat server.
    /// This is called automatically by <see cref="GetMessages"/> and <see cref="SendMessage(string,string)"/>
    /// when needed.
    /// </summary>
    /// <returns>True if the chat login succeeded, false otherwise.</returns>
    private async Task<bool> LoginChat()
    {
        var response = await _client.PostAsync(_commonData.ChatUrl, null);
        if (!response.IsSuccessStatusCode)
        {
            _logger?.LogError("Chat handshake failed: {StatusCode}", response.StatusCode);
            return false;
        }

        var json = await response.Content.ReadAsStringAsync();
        using var doc = JsonDocument.Parse(json);
        var root = doc.RootElement;

        if (!root.TryGetProperty("url", out var urlElement) || !root.TryGetProperty("auth", out var authElement))
        {
            _logger?.LogError("Chat handshake response did not contain expected url/auth fields.");
            return false;
        }

        _chatServerUrl = urlElement.GetString();
        _chatAuth = authElement.GetString();

        if (string.IsNullOrEmpty(_chatServerUrl) || string.IsNullOrEmpty(_chatAuth))
        {
            _logger?.LogError("Chat handshake returned empty url or auth.");
            return false;
        }

        _chatClient?.Dispose();
        _chatClient = new HttpClient { BaseAddress = new Uri(_chatServerUrl) };
        _chatClient.DefaultRequestHeaders.Add("auth", _chatAuth);
        return true;
    }

    /// <summary>
    /// Ensures the chat client is initialized, performing the handshake if necessary.
    /// </summary>
    private async Task<bool> EnsureChatAuthenticated()
    {
        if (_chatClient is not null && !string.IsNullOrEmpty(_chatAuth)) return true;
        return await LoginChat();
    }

    /// <summary>
    /// Retrieves recent chat conversations.
    /// </summary>
    /// <param name="max">Maximum number of chats to retrieve. Defaults to 100.</param>
    /// <returns>A list of <see cref="SpondChat"/> objects, or an empty list if none found or chat login failed.</returns>
    public async Task<List<SpondChat>> GetMessages(int max = 100)
    {
        if (!await EnsureChatAuthenticated()) return [];

        var response = await _chatClient!.GetAsync($"chats/?max={max}");
        if (!response.IsSuccessStatusCode)
        {
            _logger?.LogError("Failed to retrieve chats: {StatusCode}", response.StatusCode);
            return [];
        }
        var json = await response.Content.ReadAsStringAsync();
        return JsonConvert.DeserializeObject<List<SpondChat>>(json) ?? [];
    }

    /// <summary>
    /// Sends a message to an existing chat thread.
    /// </summary>
    /// <param name="chatId">The ID of the existing chat to send a message to.</param>
    /// <param name="text">The message text to send.</param>
    /// <returns>The sent <see cref="SpondChatMessage"/>, or null if the request failed.</returns>
    public async Task<SpondChatMessage?> SendMessage(string chatId, string text)
    {
        if (!await EnsureChatAuthenticated()) return null;

        var payload = new { chatId, text, type = MessageType.Text.ToEnumMemberValue() };
        var response = await _chatClient!.PostAsJsonAsync("messages", payload);
        if (!response.IsSuccessStatusCode)
        {
            _logger?.LogError("Failed to send message to chat {ChatId}: {StatusCode}", chatId, response.StatusCode);
            return null;
        }
        var json = await response.Content.ReadAsStringAsync();
        return JsonConvert.DeserializeObject<SpondChatMessage>(json);
    }

    /// <summary>
    /// Sends a message to a group member, starting a new chat thread.
    /// </summary>
    /// <param name="recipientProfileId">The profile ID of the recipient (the <c>Profile.Id</c> field on a member).</param>
    /// <param name="groupId">The group UID that scopes the chat.</param>
    /// <param name="text">The message text to send.</param>
    /// <returns>The sent <see cref="SpondChatMessage"/>, or null if the request failed.</returns>
    public async Task<SpondChatMessage?> SendMessage(string recipientProfileId, string groupId, string text)
    {
        if (!await EnsureChatAuthenticated()) return null;

        var payload = new { text, type = MessageType.Text.ToEnumMemberValue(), recipient = recipientProfileId, groupId };
        var response = await _chatClient!.PostAsJsonAsync("messages", payload);
        if (!response.IsSuccessStatusCode)
        {
            _logger?.LogError("Failed to start new chat with recipient {RecipientId}: {StatusCode}", recipientProfileId, response.StatusCode);
            return null;
        }
        var json = await response.Content.ReadAsStringAsync();
        return JsonConvert.DeserializeObject<SpondChatMessage>(json);
    }

    /// <summary>
    /// Retrieves Club financial transactions for the specified club.
    /// Automatically paginates to collect up to <paramref name="maxItems"/> results.
    /// </summary>
    /// <param name="clubId">The Spond Club ID (found in the Spond Club web UI URL).</param>
    /// <param name="maxItems">Maximum total number of transactions to retrieve. Defaults to 100.</param>
    /// <returns>A list of <see cref="SpondTransaction"/> objects.</returns>
    public async Task<List<SpondTransaction>> GetTransactions(string clubId, int maxItems = 100)
    {
        const string clubApiBase = "https://api.spond.com/club/v1/";
        const int pageSize = 25;
        var results = new List<SpondTransaction>();
        int skip = 0;

        using var clubClient = new HttpClient { BaseAddress = new Uri(clubApiBase) };
        foreach (var header in _client.DefaultRequestHeaders)
        {
            clubClient.DefaultRequestHeaders.TryAddWithoutValidation(header.Key, header.Value);
        }
        clubClient.DefaultRequestHeaders.TryAddWithoutValidation("X-Spond-Clubid", clubId);

        while (results.Count < maxItems)
        {
            var response = await clubClient.GetAsync($"transactions?skip={skip}");
            if (!response.IsSuccessStatusCode)
            {
                _logger?.LogError("Failed to retrieve transactions (skip={Skip}): {StatusCode}", skip, response.StatusCode);
                break;
            }
            var json = await response.Content.ReadAsStringAsync();
            var page = JsonConvert.DeserializeObject<List<SpondTransaction>>(json);
            if (page is null || page.Count == 0) break;
            results.AddRange(page);
            if (page.Count < pageSize) break;
            skip += pageSize;
        }

        return results.Take(maxItems).ToList();
    }
}
