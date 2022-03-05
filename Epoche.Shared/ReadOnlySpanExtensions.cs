namespace Epoche.Shared;
static class ReadOnlySpanExtensions
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
}
