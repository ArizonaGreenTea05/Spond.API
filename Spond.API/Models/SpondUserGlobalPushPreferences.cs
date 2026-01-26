namespace Spond.API.Models;

/// <summary>
/// Represents a user's global push notification preference settings in Spond.
/// </summary>
    public class SpondUserGlobalPushPreferences
    {
        /// <summary>
        /// Indicates whether push notifications for accepting events are disabled.
        /// </summary>
        public bool AcceptPushDisabled { get; set; }

        /// <summary>
        /// Indicates whether push notifications for declining events are disabled.
        /// </summary>
        public bool DeclinePushDisabled { get; set; }

        /// <summary>
        /// Indicates whether push notifications for accepting tasks are disabled.
        /// </summary>
        public bool AcceptTaskPushDisabled { get; set; }

        /// <summary>
        /// Indicates whether push notifications for declining tasks are disabled.
        /// </summary>
        public bool DeclineTaskPushDisabled { get; set; }

        /// <summary>
        /// Specifies the user's preferred comment notification setting.
        /// </summary>
        public string? CommentNotifications { get; set; }

        /// <summary>
        /// Indicates whether push notifications for invitations are disabled.
        /// </summary>
        public bool InvitationPushDisabled { get; set; }

        /// <summary>
        /// Indicates whether push notifications for posts are disabled.
        /// </summary>
        public bool PostPushDisabled { get; set; }

        /// <summary>
        /// Indicates whether reminder push notifications are disabled.
        /// </summary>
        public bool ReminderPushDisabled { get; set; }

        /// <summary>
        /// Indicates whether scheduled pre-alert push notifications are disabled.
        /// </summary>
        public bool ScheduledPrealertPushDisabled { get; set; }

        /// <summary>
        /// Indicates whether scheduled sent push notifications are disabled.
        /// </summary>
        public bool ScheduledSentPushDisabled { get; set; }

        /// <summary>
        /// Indicates whether push notifications for bonus contributions are disabled.
        /// </summary>
        public bool BonusContributePushDisabled { get; set; }

        /// <summary>
        /// Indicates whether push notifications for bonus achievements are disabled.
        /// </summary>
        public bool BonusAchievementPushDisabled { get; set; }

        /// <summary>
        /// Indicates whether push notifications related to match notifications are disabled.
        /// </summary>
        public bool MatchNotificationsPushDisabled { get; set; }

        /// <summary>
        /// Indicates whether push notifications for marking availability as available are disabled.
        /// </summary>
        public bool AvailablePushDisabled { get; set; }

        /// <summary>
        /// Indicates whether push notifications for marking availability as unavailable are disabled.
        /// </summary>
        public bool UnavailablePushDisabled { get; set; }

        /// <summary>
        /// Indicates whether availability reminder push notifications are disabled.
        /// </summary>
        public bool AvailabilityReminderPushDisabled { get; set; }
    }
