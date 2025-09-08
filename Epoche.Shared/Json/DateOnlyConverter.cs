using System.Text.Json;
using System.Text.Json.Serialization;

namespace Epoche.Shared.Json;

/// <summary>
/// Converts strictly to/from yyyy-MM-dd format
/// </summary>
public sealed class DateOnlyConverter : JsonConverter<DateOnly>
{
    public static readonly DateOnlyConverter Instance = new();

    public override DateOnly Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType != JsonTokenType.String)
        {
            throw new InvalidOperationException("Only string can be converted to DateOnly with this converter");
        }

        if (!reader.HasValueSequence)
        {
            var s = reader.ValueSpan;
            if (s.Length == 10 &&
                s[4] == '-' &&
                s[7] == '-')
            {
                if (s[..4].TryParseUtf8PositiveInt32(out var year) &&
                    s[5..7].TryParseUtf8PositiveInt32(out var month) &&
                    s[8..].TryParseUtf8PositiveInt32(out var day))
                {
                    return new DateOnly(year, month, day);
                }
            }
        }
        else
        {
            var s = reader.GetString();
            if (s?.Length == 10 &&
                s[4] == '-' &&
                s[7] == '-' &&
                int.TryParse(s.AsSpan(0, 4), out var year) &&
                int.TryParse(s.AsSpan(5, 2), out var month) &&
                int.TryParse(s.AsSpan(8, 2), out var day))
            {
                return new DateOnly(year, month, day);
            }
        }

        throw new FormatException("The value could not be parsed into a DateOnly");
    }

    public override void Write(Utf8JsonWriter writer, DateOnly value, JsonSerializerOptions options)
    {
        Span<byte> buf =
        [
            (byte)((value.Year / 1000) + '0'),
            (byte)(((value.Year % 1000) / 100) + '0'),
            (byte)(((value.Year % 100) / 10) + '0'),
            (byte)((value.Year % 10) + '0'),
            (byte)'-',
            (byte)(value.Month > 9 ? '1' : '0'),
            (byte)((value.Month % 10) + '0'),
            (byte)'-',
            (byte)((value.Day / 10) + '0'),
            (byte)((value.Day % 10) + '0'),
        ];
        writer.WriteStringValue(buf);
    }
}
