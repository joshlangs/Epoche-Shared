using System.Globalization;

namespace Epoche.Shared;
public class TotpHelperTests
{
    [Test]
    [Arguments("01", "Hello", "otpauth://totp/Hello?secret=AE")]
    [Arguments("0102030405060708090a0b0c0d0e0f", "Hello there!%!@#@%!#&(&fg. ", "otpauth://totp/Hello%20there%21%25%21%40%23%40%25%21%23%26%28%26fg.%20?secret=AEBAGBAFAYDQQCIKBMGA2DQP")]
    public async Task TestSecretUrl(string seed, string name, string url)
    {
        var s = TotpHelper.CreateSecretUrl(seed.ToHexBytes(), name);
        await Assert.That(s).IsEqualTo(url);
    }

    [Test]
    public async Task TestExtraIntervals()
    {
        var seed = RandomHelper.GetRandomBytes(20);
        var now = DateTimeOffset.UtcNow;
        var pw1 = TotpHelper.CalculatePassword(seed, now.AddSeconds(-30));
        var pw2 = TotpHelper.CalculatePassword(seed, now);
        var pw3 = TotpHelper.CalculatePassword(seed, now.AddSeconds(30));
        var pws = TotpHelper.CalculatePasswords(seed, now, 1).ToList();
        await Assert.That(pws.Count).IsEqualTo(3);
        await Assert.That(pws[0]).IsEqualTo(pw2);
        await Assert.That(pws).Contains(pw1);
        await Assert.That(pws).Contains(pw3);
    }

    [Test]
    [Arguments("0102030405060708090a0102030405060708090a", "Dec 12 2018 23:45:12", 743626)]
    [Arguments("0102030405060708090a0102030405060708090b", "Dec 12 2018 23:45:12", 493018)]
    [Arguments("0102030405060708090a0102030405060708090b", "Dec 12 2018 23:45:31", 576341)]
    public async Task TestCases(string seed, string when, int password)
    {
        var passwords = TotpHelper.CalculatePasswords(seed.ToHexBytes(), DateTime.Parse(when, null, DateTimeStyles.AssumeUniversal), 0).ToList();
        await Assert.That(passwords).HasSingleItem();
        await Assert.That(passwords[0]).IsEqualTo(password);
    }
}