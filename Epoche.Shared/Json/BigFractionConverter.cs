using System.Text.Json;
using System.Text.Json.Serialization;

namespace Epoche.Shared.Json;

public sealed class BigFractionConverter : JsonConverter<BigFraction>
{
    public static readonly BigFractionConverter Instance = new();

    public override BigFraction Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType != JsonTokenType.String)
        {
            throw new InvalidOperationException("Only string can be converted to BigFraction with this converter");
        }
        if (!reader.HasValueSequence && reader.ValueSpan.Length < 1024)
        {
            Span<char> str = stackalloc char[reader.ValueSpan.Length];
            if (reader.ValueSpan.TryToAsciiCharSpan(str))
            {
                if (BigFraction.TryParse(str, out var value))
                {
                    return value;
                }
            }
        }
        else if (BigFraction.TryParse(reader.GetString(), out var value))
        {
            return value;
        }
        throw new FormatException("The value could not be parsed into a BigFraction");
    }

    public override void Write(Utf8JsonWriter writer, BigFraction value, JsonSerializerOptions options)
    {
        Span<char> buf = stackalloc char[100];
        if (value.TryToCharSpan(buf, out var written))
        {
            writer.WriteStringValue(buf[..written]);
        }
        else
        {
            writer.WriteStringValue(value.ToString());
        }
    }
}