namespace Spond.API.Models;

/// <summary>
/// Represents a Spond user's notification, optional settings, and marketing preferences.
/// </summary>
public class SpondUserPreferences
{
    public SpondUserGlobalPushPreferences? GlobalPushPreferences { get; set; }

    public SpondUserGroupPushPreferences? GroupPushPreferences { get; set; }

    public SpondUserOptionalSettings? OptionalSettings { get; set; }

    public bool TargetedAdsDisabled { get; set; }

    public bool CashbackPromoDisabled { get; set; }

    public bool PartnerPromoDisabled { get; set; }
}
