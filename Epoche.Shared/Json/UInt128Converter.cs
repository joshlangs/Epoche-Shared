using System.Globalization;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Epoche.Shared.Json;

public sealed class UInt128Converter : JsonConverter<UInt128>
{
    public static readonly UInt128Converter Instance = new();

    public override UInt128 Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType != JsonTokenType.String)
        {
            throw new InvalidOperationException("Only string can be converted to UInt128 with this converter");
        }

        if (!reader.HasValueSequence && reader.ValueSpan.Length < 1024)
        {
            Span<char> str = stackalloc char[reader.ValueSpan.Length];
            if (reader.ValueSpan.TryToAsciiCharSpan(str))
            {
                if (UInt128.TryParse(str, NumberStyles.None, CultureInfo.InvariantCulture, out var value))
                {
                    return value;
                }
            }
        }
        else
        {
            var str = reader.GetString() ?? "";
            if (str.Length > 0 && str[^1] != '\0' && UInt128.TryParse(str, NumberStyles.None, CultureInfo.InvariantCulture, out var value))
            {
                return value;
            }
        }

        throw new FormatException("The value could not be parsed into a long");
    }

    public override void Write(Utf8JsonWriter writer, UInt128 value, JsonSerializerOptions options)
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
