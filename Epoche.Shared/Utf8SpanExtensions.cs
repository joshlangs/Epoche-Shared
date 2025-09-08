using System.Globalization;

namespace Epoche.Shared;
public static class Utf8SpanExtensions
{
    /// <summary>
    /// Copies a utf8 byte array into a character array as long as it contains only ascii chars (32-126).
    /// Returns false on failure, although the output buffer may have been partially written.
    /// </summary>
    public static bool TryToAsciiCharSpan(this ReadOnlySpan<byte> utf8, Span<char> buf)
    {
        if (utf8.Length > buf.Length)
        {
            return false;
        }
        for (var x = 0; x < utf8.Length; ++x)
        {
            var c = utf8[x];
            if (c < 32 || c > 126)
            {
                return false;
            }
            buf[x] = (char)c;
        }
        return true;
    }
    public static bool TryParseUtf8Int32(this ReadOnlySpan<byte> utf8, out int value)
    {
        value = 0;
        Span<char> chars = stackalloc char[utf8.Length];
        return TryToAsciiCharSpan(utf8, chars) && int.TryParse(chars, NumberStyles.AllowLeadingSign, CultureInfo.InvariantCulture, out value);
    }
    public static bool TryParseUtf8Int64(this ReadOnlySpan<byte> utf8, out long value)
    {
        value = 0;
        Span<char> chars = stackalloc char[utf8.Length];
        return TryToAsciiCharSpan(utf8, chars) && long.TryParse(chars, NumberStyles.AllowLeadingSign, CultureInfo.InvariantCulture, out value);
    }
    public static bool TryParseUtf8PositiveInt32(this ReadOnlySpan<byte> utf8, out int value)
    {
        value = 0;
        Span<char> chars = stackalloc char[utf8.Length];
        return TryToAsciiCharSpan(utf8, chars) && int.TryParse(chars, NumberStyles.None, CultureInfo.InvariantCulture, out value);
    }
    public static bool TryParseUtf8PositiveInt64(this ReadOnlySpan<byte> utf8, out long value)
    {
        value = 0;
        Span<char> chars = stackalloc char[utf8.Length];
        return TryToAsciiCharSpan(utf8, chars) && long.TryParse(chars, NumberStyles.None, CultureInfo.InvariantCulture, out value);
    }
    public static bool TryParseUtf8UInt32(this ReadOnlySpan<byte> utf8, out uint value)
    {
        value = 0;
        Span<char> chars = stackalloc char[utf8.Length];
        return TryToAsciiCharSpan(utf8, chars) && uint.TryParse(chars, NumberStyles.None, CultureInfo.InvariantCulture, out value);
    }
    public static bool TryParseUtf8UInt64(this ReadOnlySpan<byte> utf8, out ulong value)
    {
        value = 0;
        Span<char> chars = stackalloc char[utf8.Length];
        return TryToAsciiCharSpan(utf8, chars) && ulong.TryParse(chars, NumberStyles.None, CultureInfo.InvariantCulture, out value);
    }
}
