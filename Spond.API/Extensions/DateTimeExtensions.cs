namespace Spond.API.Extensions;

public static class DateTimeExtensions
{
    private static readonly string Iso8601FormatWithoutMilliseconds = "yyyy-MM-dd'T'HH:mm:ss'Z'";
    private static readonly string Iso8601FormatWithMilliseconds = "yyyy-MM-dd'T'HH:mm:ss.fff'Z'";

    public static string ToIso8601(this DateTime dateTime, bool useMilliseconds) =>
        dateTime.ToUniversalTime().ToString(useMilliseconds ? Iso8601FormatWithMilliseconds : Iso8601FormatWithoutMilliseconds);

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
