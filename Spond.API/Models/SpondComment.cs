namespace Spond.API.Models;

/// <summary>
/// Represents a comment on a Spond post.
/// </summary>
public class SpondComment
{
    /// <summary>
    /// The unique identifier of the comment.
    /// </summary>
    public string Id { get; set; } = string.Empty;

    /// <summary>
    /// The text content of the comment.
    /// </summary>
    public string Text { get; set; } = string.Empty;

    /// <summary>
    /// The ID of the author who wrote the comment.
    /// </summary>
    public string AuthorId { get; set; } = string.Empty;

    /// <summary>
    /// The timestamp when the comment was created.
    /// </summary>
    public string CreatedTime { get; set; } = string.Empty;
}
