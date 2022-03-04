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

        if (ulong.TryParse(reader.GetString(), out var value))
        {
            return value;
        }

        throw new FormatException("The value could not be parsed into a ulong");
    }

    public override void Write(Utf8JsonWriter writer, ulong value, JsonSerializerOptions options) => writer.WriteStringValue(value.ToString());
}
