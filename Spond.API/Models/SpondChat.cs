using static Spond.API.Enums;

namespace Spond.API.Models;

/// <summary>
/// Represents a chat conversation in the Spond messaging system.
/// </summary>
public class SpondChat
{
    /// <summary>
    /// The unique identifier of the chat.
    /// </summary>
    public string Id { get; set; } = string.Empty;

    /// <summary>
    /// The most recent message in the chat.
    /// </summary>
    public SpondChatMessage? Message { get; set; }
}

/// <summary>
/// Represents a single message within a Spond chat.
/// </summary>
public class SpondChatMessage
{
    /// <summary>
    /// The unique identifier of the message.
    /// </summary>
    public string Id { get; set; } = string.Empty;
    
    /// <summary>
    /// 
    /// </summary>
    [Newtonsoft.Json.JsonProperty("msgNum")]
    public int MessageNumber { get; set; }

    /// <summary>
    /// </summary>
    [Newtonsoft.Json.JsonProperty("user")]
    public string? SenderId { get; set; }

    /// <summary>
    /// The text content of the message.
    /// </summary>
    public string? Text { get; set; }

    /// <summary>
    /// The type of the message.
    /// </summary>
    public MessageType? Type { get; set; }

    /// <summary>
    /// The timestamp when the message was sent.
    /// </summary>
    public string? Timestamp { get; set; }

    /// <summary>
    /// The ID of the chat this message belongs to.
    /// </summary>
    public string? ChatId { get; set; }

    /// <summary>
    /// 
    /// </summary>
    public Dictionary<string, object?> Reactions { get; set; } = [];
}
