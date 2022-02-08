using System.Security.Cryptography;

namespace Epoche.Shared;
public static class TotpHelper
{
    public const int MinSeedLength = 16;
    public const int MaxSeedLength = 64;
    public const int DefaultSeedLength = 20;

    const int IntervalTimeInSeconds = 30;

    public static int CalculatePassword(byte[] seed) => CalculatePassword(seed, DateTimeOffset.UtcNow.ToUnixTimeSeconds());

    public static IEnumerable<int> CalculatePasswords(byte[] seed, int extraIntervals) => CalculatePasswords(seed, DateTimeOffset.UtcNow, extraIntervals);
    public static IEnumerable<int> CalculatePasswords(byte[] seed, DateTimeOffset when, int extraIntervals)
    {
        var now = when.ToUnixTimeSeconds();
        yield return CalculatePassword(seed, now);
        for (var x = 1; x <= extraIntervals; ++x)
        {
            yield return CalculatePassword(seed, now + IntervalTimeInSeconds * x);
            yield return CalculatePassword(seed, now - IntervalTimeInSeconds * x);
        }
    }
    public static int CalculatePassword(byte[] seed, DateTimeOffset when) => CalculatePassword(seed, when.ToUnixTimeSeconds());

    static int CalculatePassword(byte[] seed, long unixTimeSeconds)
    {
        if (seed is null)
        {
            throw new ArgumentNullException();
        }

        var tempArray = BitConverter.GetBytes(unixTimeSeconds / IntervalTimeInSeconds).ReverseArrayInPlace();        
        using var hmac = new HMACSHA1(seed);
        hmac.ComputeHash(tempArray);
        var hash = hmac.Hash!;
        var offset = hash[^1] & 0xf;
        return
            (
            ((hash[offset] & 0x7f) << 24) |
            (hash[offset + 1] << 16) |
            (hash[offset + 2] << 8) |
            hash[offset + 3]
            ) % 1000000;
    }

    public static string CreateSecretUrl(byte[] seed, string name) => $"otpauth://totp/{Uri.EscapeDataString(name)}?secret={Base32.Encode(seed)}";

    public static byte[] GenerateSeed(int suggestedSeedLength = DefaultSeedLength) => RandomHelper.GetRandomBytes(suggestedSeedLength < MinSeedLength ? MinSeedLength : suggestedSeedLength > MaxSeedLength ? MaxSeedLength : suggestedSeedLength);
}