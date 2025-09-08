using System.Globalization;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Epoche.Shared.Json;

public sealed class UInt64Converter : JsonConverter<ulong>
{
    public static readonly UInt64Converter Instance = new();

    public override ulong Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType != JsonTokenType.String)
        {
            throw new InvalidOperationException("Only string can be converted to ulong with this converter");
        }

        if (!reader.HasValueSequence && reader.ValueSpan.Length < 1024)
        {
            Span<char> str = stackalloc char[reader.ValueSpan.Length];
            if (reader.ValueSpan.TryToAsciiCharSpan(str))
            {
                if (ulong.TryParse(str, NumberStyles.None, CultureInfo.InvariantCulture, out var value))
                {
                    return value;
                }
            }
        }
        else
        {
            var str = reader.GetString() ?? "";
            if (str.Length > 0 && str[^1] != '\0' && ulong.TryParse(str, NumberStyles.None, CultureInfo.InvariantCulture, out var value))
            {
                return value;
            }
        }

        throw new FormatException("The value could not be parsed into a ulong");
    }

    public override void Write(Utf8JsonWriter writer, ulong value, JsonSerializerOptions options)
    {
        Span<char> buf = stackalloc char[22];
        if (value.TryFormat(buf, out var written, provider: CultureInfo.InvariantCulture))
        {
            writer.WriteStringValue(buf[..written]);
        }
        else
        {
            writer.WriteStringValue(value.ToString(CultureInfo.InvariantCulture));
        }
    }
}
