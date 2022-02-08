using System;
using System.Globalization;
using System.Linq;
using Xunit;

namespace Epoche.Shared;
public class TotpHelperTests
{
    [Theory]
    [InlineData("01", "Hello", "otpauth://totp/Hello?secret=AE")]
    [InlineData("0102030405060708090a0b0c0d0e0f", "Hello there!%!@#@%!#&(&fg. ", "otpauth://totp/Hello%20there%21%25%21%40%23%40%25%21%23%26%28%26fg.%20?secret=AEBAGBAFAYDQQCIKBMGA2DQP")]
    [Trait("Type", "Unit")]
    public void TestSecretUrl(string seed, string name, string url)
    {
        var s = TotpHelper.CreateSecretUrl(seed.ToHexBytes(), name);
        Assert.Equal(url, s);
    }

    [Fact]
    [Trait("Type", "Unit")]
    public void TestExtraIntervals()
    {
        var seed = RandomHelper.GetRandomBytes(20);
        var now = DateTimeOffset.UtcNow;
        var pw1 = TotpHelper.CalculatePassword(seed, now.AddSeconds(-30));
        var pw2 = TotpHelper.CalculatePassword(seed, now);
        var pw3 = TotpHelper.CalculatePassword(seed, now.AddSeconds(30));
        var pws = TotpHelper.CalculatePasswords(seed, now, 1).ToList();
        Assert.Equal(3, pws.Count);
        Assert.Equal(pw2, pws[0]);
        Assert.Contains(pw1, pws);
        Assert.Contains(pw3, pws);
    }

    [Theory]
    [InlineData("0102030405060708090a0102030405060708090a", "Dec 12 2018 23:45:12", 743626)]
    [InlineData("0102030405060708090a0102030405060708090b", "Dec 12 2018 23:45:12", 493018)]
    [InlineData("0102030405060708090a0102030405060708090b", "Dec 12 2018 23:45:31", 576341)]
    [Trait("Type", "Unit")]
    public void TestCases(string seed, string when, int password)
    {
        var passwords = TotpHelper.CalculatePasswords(seed.ToHexBytes(), DateTime.Parse(when, null, DateTimeStyles.AssumeUniversal), 0).ToList();
        Assert.Single(passwords);
        Assert.Equal(password, passwords[0]);
    }
}