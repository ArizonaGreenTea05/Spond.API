namespace Spond.API.Models;

/// <summary>
/// Represents a complete user profile in the Spond system.
/// </summary>
public class SpondCompleteUserProfile : SpondUserProfile
{
    /// <inheritdoc cref="SpondUserProfile.Email"/>>
    public string? PrimaryEmail
    {
        get => Email;
        set => Email = value;
    }

    public string? FormattedPhoneNumber { get; set; }

    public bool Dummy { get; set; }

    public string? TrackingId { get; set; }

    public string? Timezone { get; set; }

    public string? Locale { get; set; }

    public string? CountryCode { get; set; }

    public string? UnsubscribeCode { get; set; }

    public bool Internal { get; set; }

    public bool Deleted { get; set; }

    public int? TosVersion { get; set; }

    public bool Contact { get; set; }

    public SpondUserPreferences? Preferences { get; set; }
}