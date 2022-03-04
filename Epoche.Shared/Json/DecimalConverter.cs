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

        if (decimal.TryParse(reader.GetString(), NumberStyles.AllowDecimalPoint | NumberStyles.AllowLeadingSign, CultureInfo.InvariantCulture, out var value))
        {
            return value;
        }

        throw new FormatException("The value could not be parsed into a decimal");
    }

    public override void Write(Utf8JsonWriter writer, decimal value, JsonSerializerOptions options) => writer.WriteStringValue(value.ToString("G", CultureInfo.InvariantCulture));
}
