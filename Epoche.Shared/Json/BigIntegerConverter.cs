using System.Globalization;
using System.Numerics;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Epoche.Shared.Json;

public sealed class BigIntegerConverter : JsonConverter<BigInteger>
{
    public static readonly BigIntegerConverter Instance = new();

    public override BigInteger Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType != JsonTokenType.String)
        {
            throw new InvalidOperationException("Only string can be converted to BigInteger with this converter");
        }
        if (!reader.HasValueSequence && reader.ValueSpan.Length < 1024)
        {
            Span<char> str = stackalloc char[reader.ValueSpan.Length];
            if (reader.ValueSpan.TryToAsciiCharSpan(str) && str.Length > 0 && str[0] != '+')
            {
                if (BigInteger.TryParse(str, NumberStyles.AllowLeadingSign, CultureInfo.InvariantCulture, out var value))
                {
                    return value;
                }
            }
        }
        else
        {
            var str = reader.GetString() ?? "";
            if (str.Length > 0 && str[0] != '+' && str[^1] != '\0' && BigInteger.TryParse(str, NumberStyles.AllowLeadingSign, CultureInfo.InvariantCulture, out var value))
            {
                return value;
            }
        }

        throw new FormatException("The value could not be parsed into a BigInteger");
    }

    public override void Write(Utf8JsonWriter writer, BigInteger value, JsonSerializerOptions options)
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
