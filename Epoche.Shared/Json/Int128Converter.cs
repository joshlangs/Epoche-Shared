using System.Globalization;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Epoche.Shared.Json;

public sealed class Int128Converter : JsonConverter<Int128>
{
    public static readonly Int128Converter Instance = new();

    public override Int128 Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType != JsonTokenType.String)
        {
            throw new InvalidOperationException("Only string can be converted to Int128 with this converter");
        }

        if (!reader.HasValueSequence && reader.ValueSpan.Length < 1024)
        {
            Span<char> str = stackalloc char[reader.ValueSpan.Length];
            if (reader.ValueSpan.TryToAsciiCharSpan(str) && str.Length > 0 && str[0] != '+')
            {
                if (Int128.TryParse(str, NumberStyles.AllowLeadingSign, CultureInfo.InvariantCulture, out var value))
                {
                    return value;
                }
            }
        }
        else
        {
            var str = reader.GetString() ?? "";
            if (str.Length > 0 && str[0] != '+' && str[^1] != '\0' && Int128.TryParse(str, NumberStyles.AllowLeadingSign, CultureInfo.InvariantCulture, out var value))
            {
                return value;
            }
        }

        throw new FormatException("The value could not be parsed into a long");
    }

    public override void Write(Utf8JsonWriter writer, Int128 value, JsonSerializerOptions options)
    {
        Span<char> buf = stackalloc char[42];
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
