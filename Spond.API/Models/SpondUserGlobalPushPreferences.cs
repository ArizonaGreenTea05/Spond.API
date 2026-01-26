namespace Spond.API.Models;

/// <summary>
/// Represents a user's global push notification preference settings in Spond.
/// </summary>
public class SpondUserGlobalPushPreferences
{
    public bool AcceptPushDisabled { get; set; }

    public bool DeclinePushDisabled { get; set; }

    public bool AcceptTaskPushDisabled { get; set; }

    public bool DeclineTaskPushDisabled { get; set; }

    public string? CommentNotifications { get; set; }

    public bool InvitationPushDisabled { get; set; }

    public bool PostPushDisabled { get; set; }

    public bool ReminderPushDisabled { get; set; }

    public bool ScheduledPrealertPushDisabled { get; set; }

    public bool ScheduledSentPushDisabled { get; set; }

    public bool BonusContributePushDisabled { get; set; }

    public bool BonusAchievementPushDisabled { get; set; }

    public bool MatchNotificationsPushDisabled { get; set; }

    public bool AvailablePushDisabled { get; set; }

    public bool UnavailablePushDisabled { get; set; }

    public bool AvailabilityReminderPushDisabled { get; set; }
}
