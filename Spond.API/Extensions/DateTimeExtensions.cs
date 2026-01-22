namespace Spond.API.Extensions;

/// <summary>
/// Extension methods for DateTime to handle ISO 8601 format conversions.
/// </summary>
public static class DateTimeExtensions
{
    private static readonly string Iso8601FormatWithoutMilliseconds = "yyyy-MM-dd'T'HH:mm:ss'Z'";
    private static readonly string Iso8601FormatWithMilliseconds = "yyyy-MM-dd'T'HH:mm:ss.fff'Z'";

    /// <summary>
    /// Converts a DateTime to an ISO 8601 formatted string.
    /// </summary>
    /// <param name="dateTime">The DateTime to convert.</param>
    /// <param name="useMilliseconds">Whether to include milliseconds in the output.</param>
    /// <returns>An ISO 8601 formatted string representation of the DateTime.</returns>
    public static string ToIso8601(this DateTime dateTime, bool useMilliseconds) =>
        dateTime.ToUniversalTime().ToString(useMilliseconds ? Iso8601FormatWithMilliseconds : Iso8601FormatWithoutMilliseconds);

    /// <summary>
    /// Parses an ISO 8601 formatted string into a DateTime.
    /// </summary>
    /// <param name="input">The ISO 8601 formatted string to parse.</param>
    /// <param name="useMilliseconds">Whether the input string includes milliseconds.</param>
    /// <returns>A DateTime object representing the parsed input, converted to local time.</returns>
    public static DateTime FromIso8601(string input, bool useMilliseconds)
    {
        return DateTime.ParseExact(
            input,
            useMilliseconds ? Iso8601FormatWithMilliseconds : Iso8601FormatWithoutMilliseconds,
            System.Globalization.CultureInfo.InvariantCulture,
            System.Globalization.DateTimeStyles.AdjustToUniversal
        ).ToLocalTime();
    }
}
