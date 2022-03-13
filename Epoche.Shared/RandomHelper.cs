using System.Buffers;
using System.Security.Cryptography;

namespace Epoche.Shared;

public static class RandomHelper
{
    static readonly char[] LowerHexChars = new[] { '0', '1', '2', '3', '4', '5', '6', '7', '8', '9', 'a', 'b', 'c', 'd', 'e', 'f' };

    public static byte[] GetRandomBytes(int count) => RandomNumberGenerator.GetBytes(count);

    public static void GetRandomBytes(byte[] buf) => RandomNumberGenerator.Fill((Span<byte>)(buf ?? throw new ArgumentNullException(nameof(buf))));

    public static void GetRandomBytes(Span<byte> buf) => RandomNumberGenerator.Fill(buf);

    public static long GetRandomInt64()
    {
        Span<byte> buf = stackalloc byte[8];
        RandomNumberGenerator.Fill(buf);
        return BitConverter.ToInt64(buf);
    }

    public static int GetRandomInt32()
    {
        Span<byte> buf = stackalloc byte[4];
        RandomNumberGenerator.Fill(buf);
        return BitConverter.ToInt32(buf);
    }

    public static long GetRandomPositiveInt64()
    {
        var num = GetRandomInt64();
        if (num > 0)
        {
            return num;
        }
        num = -num;
        if (num > 0)
        {
            return num;
        }
        return GetRandomPositiveInt64();
    }

    public static int GetRandomPositiveInt32()
    {
        var num = GetRandomInt32();
        if (num > 0)
        {
            return num;
        }
        num = -num;
        if (num > 0)
        {
            return num;
        }
        return GetRandomPositiveInt32();
    }

    public static string GetRandomLowerHex(int charCount)
    {
        if (charCount < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(charCount));
        }
        if (charCount == 0)
        {
            return "";
        }
        var byteLength = (charCount + 1) / 2;
        var bytes = ArrayPool<byte>.Shared.Rent(byteLength);
        RandomNumberGenerator.Fill(bytes.AsSpan(0, byteLength));
        var str = string.Create(charCount, bytes, (chars, state) =>
        {
            var i = 0;
            var lastByteIndex = chars.Length / 2;
            for (var x = 0; x < lastByteIndex; ++x)
            {
                var b = bytes[x];
                chars[i++] = LowerHexChars[b >> 4];
                chars[i++] = LowerHexChars[b & 15];
            }
            if (i < chars.Length)
            {
                chars[i] = LowerHexChars[bytes[lastByteIndex] & 15];
            }
        });
        ArrayPool<byte>.Shared.Return(bytes);
        return str;
    }
}
