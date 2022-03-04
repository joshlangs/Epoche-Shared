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

        if (BigInteger.TryParse(reader.GetString(), NumberStyles.AllowLeadingSign, CultureInfo.InvariantCulture, out var value))
        {
            return value;
        }

        throw new FormatException("The value could not be parsed into a BigInteger");
    }

    public override void Write(Utf8JsonWriter writer, BigInteger value, JsonSerializerOptions options) => writer.WriteStringValue(value.ToString(CultureInfo.InvariantCulture));
}
