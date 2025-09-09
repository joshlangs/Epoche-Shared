using System.Globalization;

namespace Epoche.Shared;

/// <summary>
/// Because we want to use TryParseExact for any of these formats.  
/// "o" is too strict and there are no other built-in options.
/// </summary>
public static class IsoDateCheater
{
    /// <summary>
    /// DateTime.MinValue, but with Utc date type.
    /// (DateTime.MinValue == IsoDateCheater.MinValue) is true.
    /// </summary>
    public static readonly DateTime MinValue = new(0, DateTimeKind.Utc);
    /// <summary>
    /// DateTime.MaxValue, but with Utc date type.
    /// (DateTime.MaxValue == IsoDateCheater.MaxValue) is true.
    /// </summary>
    public static readonly DateTime MaxValue = new(DateTime.MaxValue.Ticks, DateTimeKind.Utc);

    static readonly string[] DecimalFormats =
    [
        "yyyy-MM-dd'T'HH:mm:ss'Z'",
        "yyyy-MM-dd'T'HH:mm:ss.f'Z'",
        "yyyy-MM-dd'T'HH:mm:ss.ff'Z'",
        "yyyy-MM-dd'T'HH:mm:ss.fff'Z'",
        "yyyy-MM-dd'T'HH:mm:ss.ffff'Z'",
        "yyyy-MM-dd'T'HH:mm:ss.fffff'Z'",
        "yyyy-MM-dd'T'HH:mm:ss.ffffff'Z'",
        "yyyy-MM-dd'T'HH:mm:ss.fffffff'Z'",
    ];
    const int DecimalIndex = 19;

    public static string GetFormat(int decimalCount)
    {
        ArgumentOutOfRangeException.ThrowIfNegative(decimalCount);
        ArgumentOutOfRangeException.ThrowIfGreaterThanOrEqual(decimalCount, DecimalFormats.Length);
        return DecimalFormats[decimalCount];
    }

    public static string[] GetFormats() => [.. DecimalFormats];

    static bool TryParseExactUtc(ReadOnlySpan<char> dateString, ReadOnlySpan<char> format, out DateTime date)
    {        
        if (DateTime.TryParseExact(dateString, format, null, DateTimeStyles.RoundtripKind, out date))
        {
            date = new DateTime(date.Ticks, DateTimeKind.Utc);
            return true;
        }
        return false;
    }

    public static bool TryParse(ReadOnlySpan<char> dateString, out DateTime date)
    {
        if (dateString.Length > DecimalIndex &&
            dateString[^1] == 'Z')
        {
            if (dateString[DecimalIndex] == 'Z')
            {
                return TryParseExactUtc(dateString, DecimalFormats[0], out date);
            }
            if (dateString[DecimalIndex] == '.')
            {
                var formatIndex = dateString.Length - DecimalIndex - 2;
                if (formatIndex < DecimalFormats.Length)
                {
                    return TryParseExactUtc(dateString, DecimalFormats[formatIndex], out date);
                }
            }
        }
        date = default;
        return false;
    }

    public static bool TryParse(string? dateString, out DateTime date) => TryParse((dateString ?? "").AsSpan(), out date);

    public static DateTime Parse(string dateString) => TryParse(dateString, out var date) ? date : throw new FormatException($"Iso date should be in a format like {DecimalFormats[^1]}");
    public static DateTime Parse(ReadOnlySpan<char> dateString) => TryParse(dateString, out var date) ? date : throw new FormatException($"Iso date should be in a format like {DecimalFormats[^1]}");
}
