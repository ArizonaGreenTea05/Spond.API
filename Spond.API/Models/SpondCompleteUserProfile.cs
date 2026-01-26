namespace Spond.API.Models;

/// <summary>
/// Represents a complete user profile in the Spond system.
/// </summary>
public class SpondCompleteUserProfile : SpondUserProfile
{
    /// <summary>
    /// Gets or sets the primary email address of the user (alias for <see cref="SpondUserProfile.Email"/>).
    /// </summary>
    public string? PrimaryEmail
    {
        get => Email;
        set => Email = value;
    }

    /// <summary>
    /// Gets or sets the formatted phone number for the user.
    /// </summary>
    public string? FormattedPhoneNumber { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether this user is a dummy or test user.
    /// </summary>
    public bool Dummy { get; set; }

    /// <summary>
    /// Gets or sets the tracking identifier associated with the user.
    /// </summary>
    public string? TrackingId { get; set; }

    /// <summary>
    /// Gets or sets the timezone identifier for the user.
    /// </summary>
    public string? Timezone { get; set; }

    /// <summary>
    /// Gets or sets the locale (language and region) preference for the user.
    /// </summary>
    public string? Locale { get; set; }

    /// <summary>
    /// Gets or sets the ISO country code associated with the user.
    /// </summary>
    public string? CountryCode { get; set; }

    /// <summary>
    /// Gets or sets the unsubscribe code used for managing the user's email preferences.
    /// </summary>
    public string? UnsubscribeCode { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the user is an internal system user.
    /// </summary>
    public bool Internal { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the user profile has been deleted.
    /// </summary>
    public bool Deleted { get; set; }

    /// <summary>
    /// Gets or sets the version of the Terms of Service the user has accepted.
    /// </summary>
    public int? TosVersion { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the user has provided contact information.
    /// </summary>
    public bool Contact { get; set; }

    /// <summary>
    /// Gets or sets the user's notification, optional settings, and marketing preferences.
    /// </summary>
    public SpondUserPreferences? Preferences { get; set; }
}