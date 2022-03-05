using System.Globalization;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Epoche.Shared.Json;

public sealed class Int64Converter : JsonConverter<long>
{
    public static readonly Int64Converter Instance = new();

    public override long Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType != JsonTokenType.String)
        {
            throw new InvalidOperationException("Only string can be converted to long with this converter");
        }

        if (!reader.HasValueSequence && reader.ValueSpan.Length < 1024)
        {
            Span<char> str = stackalloc char[reader.ValueSpan.Length];
            if (reader.ValueSpan.TryToAsciiCharSpan(str))
            {
                if (long.TryParse(str, out var value))
                {
                    return value;
                }
            }
        }
        else if (long.TryParse(reader.GetString(), NumberStyles.AllowLeadingSign, CultureInfo.InvariantCulture, out var value))
        {
            return value;
        }
        
        throw new FormatException("The value could not be parsed into a long");
    }

    public override void Write(Utf8JsonWriter writer, long value, JsonSerializerOptions options)
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
