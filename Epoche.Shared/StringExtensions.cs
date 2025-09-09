using System.Buffers;
using System.Numerics;

namespace Epoche.Shared;

public static class StringExtensions
{
    const byte InvalidHexLookup = 255;

    /// <summary>
    /// A lookup table containing 2 digit lower case hex strings for each possible byte value
    /// </summary>
    static readonly string[] LowerHexBytes = [.. Enumerable
        .Range(byte.MinValue, (byte.MaxValue - byte.MinValue) + 1)
        .Select(x => ((byte)x).ToString("x2"))];
    /// <summary>
    /// A lookup table from 0 to (int)'f' containing values from 0-15 for valid hex characters and InvalidHexLookup for invalid hex characters
    /// Example:  '4' -> 4, 'A' -> 10, 'f' -> 15, '.' -> InvalidHexLookup, 'G' -> InvalidHexLookup, 'g' -> (out of range)
    /// </summary>
    static readonly byte[] HexLookup = [.. Enumerable
        .Range(byte.MinValue, 'f' + 1)
        .Select(x => x >= '0' && x <= '9' ? x - '0' : x >= 'a' && x <= 'f' ? 10 + x - 'a' : x >= 'A' && x <= 'F' ? 10 + x - 'A' : InvalidHexLookup)
        .Select(x => (byte)x)];

    /// <summary>
    /// Converts a byte array to lower case hex characters. String length always a multiple of 2.
    /// </summary>
    public static string ToLowerHex(this byte[] data) => Convert.ToHexStringLower(data);

    /// <summary>
    /// Converts bytes to lower case hex characters. String length always a multiple of 2.
    /// </summary>
    public static string ToLowerHex(this Memory<byte> data) => Convert.ToHexStringLower(data.Span);

    /// <summary>
    /// Converts bytes to lower case hex characters. String length always a multiple of 2.
    /// </summary>
    public static string ToLowerHex(this ReadOnlyMemory<byte> data) => Convert.ToHexStringLower(data.Span);

    /// <summary>
    /// Converts a byte array to lower case hex characters. String length always a multiple of 2.
    /// </summary>
    public static string ToLowerHex(this Span<byte> data) => Convert.ToHexStringLower(data);

    /// <summary>
    /// Converts a byte array to lower case hex characters. String length always a multiple of 2.
    /// </summary>
    public static string ToLowerHex(this ReadOnlySpan<byte> data) => Convert.ToHexStringLower(data);

    public static BigInteger? TryToBigInteger(this string s) => BigInteger.TryParse(s, out var val) ? val : null;
    public static BigFraction? TryToBigFraction(this string value) => BigFraction.TryParse(value, out var val) ? val : null;
    public static long? TryToInt64(this string s) => long.TryParse(s, out var val) ? val : null;
    public static int? TryToInt32(this string s) => int.TryParse(s, out var val) ? val : null;
    public static double? TryToDouble(this string s) => double.TryParse(s, out var val) ? val : null;
    public static decimal? TryToDecimal(this string s) => decimal.TryParse(s, out var val) ? val : null;

    /// <summary>
    /// Converts a string of hex characters into a byte array
    /// The string length must be a multiple of 2 and contain only hex characters
    /// </summary>
    public static byte[] ToHexBytes(this string data) => Convert.FromHexString(data);

    /// <summary>
    /// Converts a string of hex characters into a byte array
    /// The string length must be a multiple of 2 and contain only hex characters
    /// </summary>
    public static void ToHexBytes(this string data, Span<byte> destination)
    {
        ArgumentNullException.ThrowIfNull(data);
        ToHexBytes(data.AsSpan(), destination);
    }

    /// <summary>
    /// Converts a string of hex characters into a byte array
    /// The string length must be a multiple of 2 and contain only hex characters
    /// </summary>
    public static void ToHexBytes(this ReadOnlySpan<char> data, Span<byte> destination)
    {
        var result = Convert.FromHexString(data, destination, out _, out _);
        if (result == OperationStatus.Done)
        {
            return;
        }
        if (result == OperationStatus.DestinationTooSmall)
        {
            throw new ArgumentOutOfRangeException(nameof(destination));
        }
        throw new ArgumentOutOfRangeException(nameof(data));
    }

    /// <summary>
    /// Test if a string of characters are hex and can be converted into a byte array.
    /// Returns true if the string is empty (it would be converted into a zero length byte array).
    /// </summary>
    public static bool IsHexBytes(this string? data)
    {
        if (data is null || data.Length % 2 != 0)
        {
            return false;
        }
        for (var x = 0; x < data.Length; ++x)
        {
            var c = data[x];
            if ((c >= '0' && c <= '9') || (c >= 'a' && c <= 'f') || (c >= 'A' && c <= 'F'))
            {
                continue;
            }
            return false;
        }
        return true;
    }

    /// <summary>
    /// Converts a string of hex characters into a byte array
    /// The string length must be a multiple of 2 and contain only hex characters
    /// On failure, returns null instead of throwing an exception
    /// </summary>
    public static byte[]? TryToHexBytes(this string? data)
    {
        if (data is null)
        {
            return null;
        }
        return TryToHexBytes(data.AsSpan());
    }

    /// <summary>
    /// Converts a string of hex characters into a byte array
    /// The string length must be a multiple of 2 and contain only hex characters
    /// On failure, returns null instead of throwing an exception
    /// </summary>
    public static byte[]? TryToHexBytes(this ReadOnlySpan<char> data)
    {
        if (data.Length % 2 != 0)
        {
            return null;
        }
        var buf = new byte[data.Length / 2];
        var result = Convert.FromHexString(data, buf, out _, out _);
        if (result == OperationStatus.Done)
        {
            return buf;
        }
        return null;
    }
    public static string? NullIfEmpty(this string? str) => string.IsNullOrEmpty(str) ? null : str;

    public static void ToLowerHexUtf8(this Span<byte> data, Span<byte> hexUtf8String) => ToLowerHexUtf8((ReadOnlySpan<byte>)data, hexUtf8String);
    public static void ToLowerHexUtf8(this ReadOnlySpan<byte> data, Span<byte> hexUtf8String)
    {
        if (data.Length * 2 != hexUtf8String.Length)
        {
            throw new ArgumentOutOfRangeException(nameof(hexUtf8String));
        }
        var hexIndex = hexUtf8String.Length - 1;
        for (var dataIndex = data.Length - 1; dataIndex >= 0; --dataIndex)
        {
            var b = data[dataIndex];
            var n = b & 0xf;
            hexUtf8String[hexIndex--] = (byte)(n < 10 ? '0' + n : 'a' - 10 + n);
            n = b >> 4;
            hexUtf8String[hexIndex--] = (byte)(n < 10 ? '0' + n : 'a' - 10 + n);
        }
    }

    public static void ToLowerHexUtf8(this Span<byte> data, Span<char> hexUtf8String) => ToLowerHexUtf8((ReadOnlySpan<byte>)data, hexUtf8String);
    public static void ToLowerHexUtf8(this ReadOnlySpan<byte> data, Span<char> hexUtf8String)
    {
        if (data.Length * 2 != hexUtf8String.Length)
        {
            throw new ArgumentOutOfRangeException(nameof(hexUtf8String));
        }
        var hexIndex = hexUtf8String.Length - 1;
        for (var dataIndex = data.Length - 1; dataIndex >= 0; --dataIndex)
        {
            var b = data[dataIndex];
            var n = b & 0xf;
            hexUtf8String[hexIndex--] = (char)(n < 10 ? '0' + n : 'a' - 10 + n);
            n = b >> 4;
            hexUtf8String[hexIndex--] = (char)(n < 10 ? '0' + n : 'a' - 10 + n);
        }
    }

    public static void ToBytesFromHexUtf8(this Span<byte> hexUtf8String, Span<byte> data) => ToBytesFromHexUtf8((ReadOnlySpan<byte>)hexUtf8String, data);
    public static void ToBytesFromHexUtf8(this ReadOnlySpan<byte> hexUtf8String, Span<byte> data)
    {
        if (data.Length * 2 != hexUtf8String.Length)
        {
            throw new ArgumentOutOfRangeException(nameof(hexUtf8String));
        }

        var hexIndex = hexUtf8String.Length - 1;
        for (var dataIndex = data.Length - 1; dataIndex >= 0; --dataIndex)
        {
            var b = hexUtf8String[hexIndex--];
            var hex = (byte)
                (
                b >= '0' && b <= '9'
                ? b - '0'
                : b >= 'a' && b <= 'f'
                ? b - 'a' + 10
                : b >= 'A' && b <= 'F'
                ? b - 'A' + 10
                : throw new FormatException()
                );
            b = hexUtf8String[hexIndex--];
            data[dataIndex] = (byte)
                (((
                b >= '0' && b <= '9'
                ? b - '0'
                : b >= 'a' && b <= 'f'
                ? b - 'a' + 10
                : b >= 'A' && b <= 'F'
                ? b - 'A' + 10
                : throw new FormatException()
                ) << 4) | hex);
        }
    }

    public static void ToBytesFromHexUtf8(this Span<byte> hexUtf8String, Span<char> data) => ToBytesFromHexUtf8((ReadOnlySpan<byte>)hexUtf8String, data);
    public static void ToBytesFromHexUtf8(this ReadOnlySpan<byte> hexUtf8String, Span<char> data)
    {
        if (data.Length * 2 != hexUtf8String.Length)
        {
            throw new ArgumentOutOfRangeException(nameof(hexUtf8String));
        }

        var hexIndex = hexUtf8String.Length - 1;
        for (var dataIndex = data.Length - 1; dataIndex >= 0; --dataIndex)
        {
            var b = hexUtf8String[hexIndex--];
            var hex = (byte)
                (
                b >= '0' && b <= '9'
                ? b - '0'
                : b >= 'a' && b <= 'f'
                ? b - 'a' + 10
                : b >= 'A' && b <= 'F'
                ? b - 'A' + 10
                : throw new FormatException()
                );
            b = hexUtf8String[hexIndex--];
            data[dataIndex] = (char)
                (((
                b >= '0' && b <= '9'
                ? b - '0'
                : b >= 'a' && b <= 'f'
                ? b - 'a' + 10
                : b >= 'A' && b <= 'F'
                ? b - 'A' + 10
                : throw new FormatException()
                ) << 4) | hex);
        }
    }

    public static void ToBytesFromHexUtf8(this ReadOnlySequence<byte> hexUtf8String, Span<byte> data)
    {
        if (data.Length * 2 != hexUtf8String.Length)
        {
            throw new ArgumentOutOfRangeException(nameof(hexUtf8String));
        }

        var pos = hexUtf8String.Start;
        var dataIndex = 0;
        byte current = 0;
        var left = true;
        while (dataIndex < data.Length)
        {
            if (!hexUtf8String.TryGet(ref pos, out var memory))
            {
                throw new FormatException("hexUtf8String.TryGet returned false before end of string");
            }
            var span = memory.Span;
            for (var x = 0; x < span.Length; ++x)
            {
                var b = span[x];
                var hex = (byte)
                    (
                    b >= '0' && b <= '9'
                    ? b - '0'
                    : b >= 'a' && b <= 'f'
                    ? b - 'a' + 10
                    : b >= 'A' && b <= 'F'
                    ? b - 'A' + 10
                    : throw new FormatException()
                    );
                if (left)
                {
                    current = (byte)(hex << 4);
                    left = false;
                }
                else
                {
                    data[dataIndex++] = (byte)(current | hex);
                    left = true;
                }
            }
        }
        if (dataIndex < data.Length)
        {
            throw new FormatException("Ran out of bytes before finishing ToBytesFromHexUtf8");
        }
    }

    public static void ToBytesFromHexUtf8(this ReadOnlySequence<char> hexUtf8String, Span<byte> data)
    {
        if (data.Length * 2 != hexUtf8String.Length)
        {
            throw new ArgumentOutOfRangeException(nameof(hexUtf8String));
        }

        var pos = hexUtf8String.Start;
        var dataIndex = 0;
        byte current = 0;
        var left = true;
        while (dataIndex < data.Length)
        {
            if (!hexUtf8String.TryGet(ref pos, out var memory))
            {
                throw new FormatException("hexUtf8String.TryGet returned false before end of string");
            }
            var span = memory.Span;
            for (var x = 0; x < span.Length; ++x)
            {
                var b = span[x];
                var hex = (byte)
                    (
                    b >= '0' && b <= '9'
                    ? b - '0'
                    : b >= 'a' && b <= 'f'
                    ? b - 'a' + 10
                    : b >= 'A' && b <= 'F'
                    ? b - 'A' + 10
                    : throw new FormatException()
                    );
                if (left)
                {
                    current = (byte)(hex << 4);
                    left = false;
                }
                else
                {
                    data[dataIndex++] = (byte)(current | hex);
                    left = true;
                }
            }
        }
        if (dataIndex < data.Length)
        {
            throw new FormatException("Ran out of bytes before finishing ToBytesFromHexUtf8");
        }
    }

    public static string Truncate(this string s, int maxLength)
    {
        ArgumentOutOfRangeException.ThrowIfNegative(maxLength);
        return s.Length > maxLength ? s[..maxLength] : s;
    }
    public static string Truncate(this string s, int startingIndex, int maxLength)
    {
        ArgumentOutOfRangeException.ThrowIfNegative(startingIndex);
        ArgumentOutOfRangeException.ThrowIfGreaterThan(startingIndex, s.Length);
        ArgumentOutOfRangeException.ThrowIfNegative(maxLength);
        return
            (long)startingIndex + maxLength > int.MaxValue ? s[startingIndex..] :
            startingIndex + maxLength < s.Length ? s.Substring(startingIndex, maxLength) :
            s[startingIndex..];
    }
}
