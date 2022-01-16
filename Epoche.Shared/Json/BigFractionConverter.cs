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
        if (BigFraction.TryParse(reader.GetString(), out var value))
        {
            return value;
        }
        throw new FormatException("The value could not be parsed into a BigFraction");
    }

    public override void Write(Utf8JsonWriter writer, BigFraction value, JsonSerializerOptions options) => writer.WriteStringValue(value.ToString());
}
