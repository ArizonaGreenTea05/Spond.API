namespace Spond.API.Models;

/// <summary>
/// Represents a Spond user's notification, optional settings, and marketing preferences.
/// </summary>
public class SpondUserPreferences
{
    /// <summary>
/// Gets or sets the user's global push notification preferences that apply across all groups and activities.
/// </summary>
    public SpondUserGlobalPushPreferences? GlobalPushPreferences { get; set; }

    /// <summary>
/// Gets or sets the user's push notification preferences specific to individual groups.
/// </summary>
    public SpondUserGroupPushPreferences? GroupPushPreferences { get; set; }

    /// <summary>
/// Gets or sets the user's optional account and application settings.
/// </summary>
    public SpondUserOptionalSettings? OptionalSettings { get; set; }

    /// <summary>
/// Gets or sets a value indicating whether targeted advertising is disabled for the user.
/// </summary>
    public bool TargetedAdsDisabled { get; set; }

    /// <summary>
/// Gets or sets a value indicating whether cashback promotional messages are disabled for the user.
/// </summary>
    public bool CashbackPromoDisabled { get; set; }

    /// <summary>
/// Gets or sets a value indicating whether partner promotional messages are disabled for the user.
/// </summary>
    public bool PartnerPromoDisabled { get; set; }
}
