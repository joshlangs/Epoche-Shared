using System.Text.Json;
using System.Text.Json.Serialization;

namespace Epoche.Shared.Json;

/// <summary>
/// Always outputs 7 digits of fractional seconds (ISO 8601).
/// Always reads DateTime in UTC.
/// Local DateTime are written after being converted to UTC.  Unspecified DateTime kinds are assumed to be UTC.
/// But can read 0-7 fractional seconds (as per IsoDateCheater)
/// https://docs.microsoft.com/en-us/dotnet/standard/base-types/standard-date-and-time-format-strings#the-round-trip-o-o-format-specifier
/// Note: Not as flexible as https://docs.microsoft.com/en-us/dotnet/standard/datetime/system-text-json-support#the-extended-iso-8601-12019-profile-in-systemtextjson which takes 8+ fractional seconds and potentially other formats.
/// </summary>
public sealed class IsoDateTimeConverter : JsonConverter<DateTime>
{
    public static readonly IsoDateTimeConverter Instance = new();

    public override DateTime Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType != JsonTokenType.String)
        {
            throw new InvalidOperationException("Only string can be converted to DateTime with this converter");
        }

        DateTime value;
        if (!reader.HasValueSequence)
        {
            Span<char> str = stackalloc char[reader.ValueSpan.Length];
            if (reader.ValueSpan.TryToAsciiCharSpan(str))
            {
                if (IsoDateCheater.TryParse(str, out value))
                {
                    return value;
                }
            }
        }
        else if (IsoDateCheater.TryParse(reader.GetString(), out value))
        {
            return value;
        }

        throw new FormatException("The value could not be parsed into a DateTime");
    }

    public override void Write(Utf8JsonWriter writer, DateTime value, JsonSerializerOptions options)
    {
        Span<char> buf = stackalloc char[28];
        if (value.Kind == DateTimeKind.Unspecified)
        {
            value = new DateTime(value.Ticks, DateTimeKind.Utc);
        }
        else if (value.Kind == DateTimeKind.Local)
        {
            value = value.ToUniversalTime();
        }
        if (value.TryFormat(buf, out var written, "O"))
        {
            writer.WriteStringValue(buf[..written]);
        }
        else
        {
            writer.WriteStringValue(value.ToString("O"));
        }
    }
}
