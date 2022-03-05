using System.Globalization;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Epoche.Shared.Json;

public sealed class DecimalConverter : JsonConverter<decimal>
{
    public static readonly DecimalConverter Instance = new();

    public override decimal Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType != JsonTokenType.String)
        {
            throw new InvalidOperationException("Only string can be converted to decimal with this converter");
        }
        if (!reader.HasValueSequence && reader.ValueSpan.Length < 1024)
        {
            Span<char> str = stackalloc char[reader.ValueSpan.Length];
            if (reader.ValueSpan.TryToAsciiCharSpan(str))
            {
                if (decimal.TryParse(str, out var value))
                {
                    return value;
                }
            }
        }
        else if (decimal.TryParse(reader.GetString(), NumberStyles.AllowDecimalPoint | NumberStyles.AllowLeadingSign, CultureInfo.InvariantCulture, out var value))
        {
            return value;
        }

        throw new FormatException("The value could not be parsed into a decimal");
    }

    public override void Write(Utf8JsonWriter writer, decimal value, JsonSerializerOptions options)
    {
        Span<char> buf = stackalloc char[100];
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
