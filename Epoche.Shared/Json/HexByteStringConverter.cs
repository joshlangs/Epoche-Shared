using System.Text.Json;
using System.Text.Json.Serialization;

namespace Epoche.Shared.Json;

/// <summary>
/// This will read strings as lowercase hex characters, stripping leading '0x' and ensuring the length is as expected (by default, exactly 32 bytes / 64 characters).
/// When serializing, strings are written as-is and without validation.
/// </summary>
/// <param name="allowEmpty">If true, the empty string is allowed, but not "0x"</param>
public sealed class HexByteStringConverter(int minBytes, int maxBytes, bool allowEmpty) : JsonConverter<string>
{
    public static readonly HexByteStringConverter Default = new();

    readonly int MinLength = minBytes * 2;
    readonly int MaxLength = maxBytes * 2;
    readonly bool AllowEmpty = allowEmpty;

    public HexByteStringConverter() : this(32, 32, false) { }

    public override string Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType != JsonTokenType.String)
        {
            throw new InvalidOperationException("Only string can be read with this converter");
        }
        var hash = reader.GetString()!;
        if (AllowEmpty && hash.Length == 0) { return ""; }
        if (hash.Length % 2 == 0)
        {
            if (hash.StartsWith("0x"))
            {
                hash = hash[2..];
            }
            if (hash.Length >= MinLength && hash.Length <= MaxLength)
            {
                if (hash.IsHexBytes())
                {
                    return hash.ToLower();
                }
            }
        }
        throw new FormatException($"The value {hash} is not a valid transaction hash");
    }

    public override void Write(Utf8JsonWriter writer, string value, JsonSerializerOptions options) => writer.WriteStringValue(value);
}
