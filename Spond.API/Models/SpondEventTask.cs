using static Spond.API.Enums;

namespace Spond.API.Models;

/// <summary>
/// Represents an assigned task within a Spond event.
/// </summary>
public class SpondEventAssignedTask
{
    /// <summary>
    /// The unique identifier of the task.
    /// </summary>
    public string? Id { get; set; }

    /// <summary>
    /// The name of the task.
    /// </summary>
    public string? Name { get; set; }

    /// <summary>
    /// The description of the task.
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// The type of the task.
    /// </summary>
    public TaskType? Type { get; set; }

    /// <summary>
    /// Indicates whether the task is restricted to adults only.
    /// </summary>
    public bool AdultsOnly { get; set; }

    /// <summary>
    /// The assignment details for this task.
    /// </summary>
    public SpondEventTaskAssignments? Assignments { get; set; }
}

/// <summary>
/// Represents the assignments for an event task.
/// </summary>
public class SpondEventTaskAssignments
{
    /// <summary>
    /// The list of member IDs assigned to this task.
    /// </summary>
    public List<string> MemberIds { get; set; } = [];

    /// <summary>
    /// The list of profile IDs assigned to this task.
    /// </summary>
    public List<string> Profiles { get; set; } = [];
}

/// <summary>
/// Represents the tasks container for an event.
/// </summary>
public class SpondEventTasks
{
    /// <summary>
    /// Open (unassigned) tasks for the event.
    /// </summary>
    public List<object> OpenTasks { get; set; } = [];

    /// <summary>
    /// Tasks that have been assigned to specific members.
    /// </summary>
    public List<SpondEventAssignedTask> AssignedTasks { get; set; } = [];
}
