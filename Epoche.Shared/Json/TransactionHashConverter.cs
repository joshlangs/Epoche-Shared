using System.Text.Json;
using System.Text.Json.Serialization;

namespace Epoche.Shared.Json;

/// <summary>
/// This will read strings as lowercase hex characters, stripping leading '0x' and ensuring the length is as expected.
/// When serializing, strings are written as-is and without validation.
/// </summary>
public sealed class TransactionHashConverter : JsonConverter<string>
{
    public static readonly TransactionHashConverter Default = new(32, 32, false);

    readonly int MinLength;
    readonly int MaxLength;
    readonly bool AllowEmpty;

    /// <param name="allowEmpty">If true, the empty string is allowed, but not "0x"</param>
    public TransactionHashConverter(int minBytes, int maxBytes, bool allowEmpty)
    {
        MinLength = minBytes * 2;
        MaxLength = maxBytes * 2;
        AllowEmpty = allowEmpty;
    }

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
                if (hash.TryToHexBytes() is not null)
                {
                    return hash.ToLower();
                }
            }
        }
        throw new FormatException($"The value {hash} is not a valid transaction hash");
    }

    public override void Write(Utf8JsonWriter writer, string value, JsonSerializerOptions options) => writer.WriteStringValue(value);
}
