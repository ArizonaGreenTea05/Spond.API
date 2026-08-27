using static Spond.API.Enums;

namespace Spond.API.Models;

/// <summary>
/// Represents a post on a Spond group wall.
/// </summary>
public class SpondPost
{
    /// <summary>
    /// The unique identifier of the post.
    /// </summary>
    public string Id { get; set; } = string.Empty;

    /// <summary>
    /// The text content of the post.
    /// </summary>
    public string Text { get; set; } = string.Empty;

    /// <summary>
    /// The type of the post.
    /// </summary>
    public PostType? Type { get; set; }

    /// <summary>
    /// The ID of the group this post belongs to.
    /// </summary>
    public string? GroupId { get; set; }

    /// <summary>
    /// The author of the post.
    /// </summary>
    public SpondMember? Author { get; set; }

    /// <summary>
    /// The timestamp when the post was created.
    /// </summary>
    public string CreatedTime { get; set; } = string.Empty;

    /// <summary>
    /// The comments on this post.
    /// </summary>
    public List<SpondComment> Comments { get; set; } = [];
}
