namespace Epoche.Shared;

public class IsoDateCheaterTests
{
    static readonly string[] Formats =
    [
        "yyyy-MM-dd'T'HH:mm:ss'Z'",
        "yyyy-MM-dd'T'HH:mm:ss.f'Z'",
        "yyyy-MM-dd'T'HH:mm:ss.ff'Z'",
        "yyyy-MM-dd'T'HH:mm:ss.fff'Z'",
        "yyyy-MM-dd'T'HH:mm:ss.ffff'Z'",
        "yyyy-MM-dd'T'HH:mm:ss.fffff'Z'",
        "yyyy-MM-dd'T'HH:mm:ss.ffffff'Z'",
        "yyyy-MM-dd'T'HH:mm:ss.fffffff'Z'"
    ];

    [Test]
    public async Task GetFormats_ReturnsExpectedValues() => await Assert.That(IsoDateCheater.GetFormats()).IsEquivalentTo(Formats);

    [Test]
    public async Task GetFormat_ReturnsExpectedValues()
    {
        for (var x = 0; x < Formats.Length; ++x)
        {
            await Assert.That(IsoDateCheater.GetFormat(x)).IsEqualTo(Formats[x]);
        }
    }

    [Test]
    public async Task GetFormat_Negative_ThrowsArgumentOutOfRangeException() => Assert.Throws<ArgumentOutOfRangeException>(() => IsoDateCheater.GetFormat(-1));

    [Test]
    public async Task GetFormat_TooHigh_ThrowsArgumentOutOfRangeException() => Assert.Throws<ArgumentOutOfRangeException>(() => IsoDateCheater.GetFormat(-1));

    [Test]
    public async Task TryParse_EachFormat_ResultsMatch()
    {
        for (var x = 0; x < Formats.Length; ++x)
        {
            var now = DateTime.UtcNow;
            var nowString = now.ToString(Formats[x]);
            await Assert.That(IsoDateCheater.TryParse(nowString, out var date)).IsTrue();
            await Assert.That(date.ToString(Formats[x])).IsEqualTo(nowString);
        }
    }

    [Test]
    public async Task TryParse_EachFormat_SpanResultsMatch()
    {
        for (var x = 0; x < Formats.Length; ++x)
        {
            var now = DateTime.UtcNow;
            var nowString = now.ToString(Formats[x]);
            await Assert.That(IsoDateCheater.TryParse(nowString.AsSpan(), out var date)).IsTrue();
            await Assert.That(date.ToString(Formats[x])).IsEqualTo(nowString);
        }
    }

    [Test]
    public async Task TryParse_Null_ReturnsFalse() => await Assert.That(IsoDateCheater.TryParse(null, out _)).IsFalse();

    [Test]
    public async Task TryParse_InvalidText_ReturnsFalse() => await Assert.That(IsoDateCheater.TryParse("asdfoiasfd", out _)).IsFalse();

    [Test]
    public async Task TryParse_ValidDateWrongFormat_ReturnsFalse() => await Assert.That(IsoDateCheater.TryParse(DateTime.UtcNow.ToString("u"), out _)).IsFalse();

    [Test]
    public async Task TryParse_o_ReturnsTrue() => await Assert.That(IsoDateCheater.TryParse(DateTime.UtcNow.ToString("O"), out _)).IsTrue();

    [Test]
    public async Task Parse_ToString_RoundtripWorks()
    {
        var date = DateTime.UtcNow;
        var newDate = IsoDateCheater.Parse(date.ToString("O"));
        await Assert.That(date).IsEqualTo(newDate);
        await Assert.That(date.ToString("O")).IsEqualTo(newDate.ToString("O"));
    }

    [Test]
    public async Task Parse_ToString_SpanRoundtripWorks()
    {
        var date = DateTime.UtcNow;
        var newDate = IsoDateCheater.Parse(date.ToString("O").AsSpan());
        await Assert.That(date).IsEqualTo(newDate);
        await Assert.That(date.ToString("O")).IsEqualTo(newDate.ToString("O"));
    }

    [Test]
    public async Task Parse_ReturnsUtc()
    {
        var newDate = IsoDateCheater.Parse(DateTime.UtcNow.ToString("O"));
        await Assert.That(newDate.Kind).IsEqualTo(DateTimeKind.Utc);
    }

    [Test]
    public async Task MinValue_0UTC()
    {
        await Assert.That(IsoDateCheater.MinValue.Ticks).IsEqualTo(0);
        await Assert.That(IsoDateCheater.MinValue.Kind).IsEqualTo(DateTimeKind.Utc);
    }

    [Test]
    public async Task MaxValue_MaxUTC()
    {
        await Assert.That(IsoDateCheater.MaxValue.Ticks).IsEqualTo(DateTime.MaxValue.Ticks);
        await Assert.That(IsoDateCheater.MaxValue.Kind).IsEqualTo(DateTimeKind.Utc);
    }

    [Test]
    public async Task MinValue_Equivalent() => await Assert.That(IsoDateCheater.MinValue).IsEqualTo(DateTime.MinValue);

    [Test]
    public async Task MaxValue_Equivalent() => await Assert.That(IsoDateCheater.MaxValue).IsEqualTo(DateTime.MaxValue);
}
