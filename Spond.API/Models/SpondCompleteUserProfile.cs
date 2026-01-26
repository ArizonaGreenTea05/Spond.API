namespace Spond.API.Models;

/// <summary>
/// Represents a complete user profile in the Spond system.
/// </summary>
public class SpondCompleteUserProfile : SpondUserProfile
{
    /// <summary>
    /// The primary email address of the user (alias for <see cref="SpondUserProfile.Email"/>).
    /// </summary>
    public string? PrimaryEmail
    {
        get => Email;
        set => Email = value;
    }

    /// <summary>
    /// The formatted phone number for the user.
    /// </summary>
    public string? FormattedPhoneNumber { get; set; }

    /// <summary>
    /// Indicates whether this user is a dummy or test user.
    /// </summary>
    public bool Dummy { get; set; }

    /// <summary>
    /// The tracking identifier associated with the user.
    /// </summary>
    public string? TrackingId { get; set; }

    /// <summary>
    /// The timezone identifier for the user.
    /// </summary>
    public string? Timezone { get; set; }

    /// <summary>
    /// The locale (language and region) preference for the user.
    /// </summary>
    public string? Locale { get; set; }

    /// <summary>
    /// The ISO country code associated with the user.
    /// </summary>
    public string? CountryCode { get; set; }

    /// <summary>
    /// The unsubscribe code used for managing the user's email preferences.
    /// </summary>
    public string? UnsubscribeCode { get; set; }

    /// <summary>
    /// Indicates whether the user is an internal system user.
    /// </summary>
    public bool Internal { get; set; }

    /// <summary>
    /// Indicates whether the user profile has been deleted.
    /// </summary>
    public bool Deleted { get; set; }

    /// <summary>
    /// The version of the Terms of Service the user has accepted.
    /// </summary>
    public int? TosVersion { get; set; }

    /// <summary>
    /// Indicates whether the user has provided contact information.
    /// </summary>
    public bool Contact { get; set; }

    /// <summary>
    /// The user's notification, optional settings, and marketing preferences.
    /// </summary>
    public SpondUserPreferences? Preferences { get; set; }
}