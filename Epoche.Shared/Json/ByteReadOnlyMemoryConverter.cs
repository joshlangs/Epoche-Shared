using System.Text.Json;
using System.Text.Json.Serialization;

namespace Epoche.Shared.Json;

public sealed class ByteReadOnlyMemoryConverter : JsonConverter<ReadOnlyMemory<byte>>
{
    public static readonly ByteArrayConverter Instance = new();

    public override ReadOnlyMemory<byte> Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType != JsonTokenType.String)
        {
            throw new InvalidOperationException("Only string can be converted to ReadOnlyMemory<byte> with this converter");
        }

        if (reader.HasValueSequence)
        {
            var hexSequence = reader.ValueSequence;
            if (hexSequence.Length == 0)
            {
                return ReadOnlyMemory<byte>.Empty;
            }
            var data = new byte[hexSequence.Length / 2];
            hexSequence.ToBytesFromHexUtf8(data);
            return new(data);
        }
        else
        {
            var hexSpan = reader.ValueSpan;
            if (hexSpan.Length == 0)
            {
                return ReadOnlyMemory<byte>.Empty;
            }
            var data = new byte[hexSpan.Length / 2];
            hexSpan.ToBytesFromHexUtf8(data);
            return new(data);
        }
    }

    public override void Write(Utf8JsonWriter writer, ReadOnlyMemory<byte> value, JsonSerializerOptions options)
    {
        if (value.Length == 0)
        {
            writer.WriteStringValue("");
            return;
        }
        Span<byte> hex =
            value.Length <= 1024
            ? stackalloc byte[value.Length * 2]
            : new byte[value.Length * 2];
        value.Span.ToLowerHexUtf8(hex);
        writer.WriteStringValue(hex);
    }
}
