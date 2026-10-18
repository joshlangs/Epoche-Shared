namespace Epoche.Shared;
public class Utf8SpanExtensionsTests
{
    static ReadOnlySpan<byte> StringToBytes(string s)
    {
        var buf = new byte[s.Length];
        for (var x = 0; x < buf.Length; ++x)
        {
            buf[x] = (byte)s[x];
        }
        return buf;
    }

    [Test]
    public async Task TryToAsciiCharSpan_Empty_True() => await Assert.That(((ReadOnlySpan<byte>)[]).TryToAsciiCharSpan([])).IsTrue();

    [Test]
    public async Task TryToAsciiCharSpan_TooShort_False() => await Assert.That(((ReadOnlySpan<byte>)new byte[1].AsSpan()).TryToAsciiCharSpan([])).IsFalse();

    [Test]
    public async Task TryToAsciiCharSpan_32_126_True()
    {
        var buf = new byte[126 - 32 + 1];
        for (var x = 0; x < buf.Length; ++x)
        {
            buf[x] = (byte)(32 + x);
        }

        var chars = new char[buf.Length];
        await Assert.That(((ReadOnlySpan<byte>)buf.AsSpan()).TryToAsciiCharSpan(chars)).IsTrue();
        for (var x = 0; x < buf.Length; ++x)
        {
            await Assert.That((char)buf[x]).IsEqualTo(chars[x]);
        }
    }

    [Test]
    public async Task TryToAsciiCharSpan_NotAscii_False()
    {
        var buf = new byte[1];
        var chars = new char[1];
        List<bool> results = new(128 + 32);
        for (var x = 0; x < 32; ++x)
        {
            buf[0] = (byte)x;
            await Assert.That(buf.AsSpan().TryToAsciiCharSpan(chars)).IsFalse();
        }
        for (var x = 127; x <= byte.MaxValue; ++x)
        {
            buf[0] = (byte)x;
            await Assert.That(buf.AsSpan().TryToAsciiCharSpan(chars)).IsFalse();
        }
    }

    [Test]
    [Arguments("")]
    [Arguments("-")]
    [Arguments(".")]
    [Arguments(".-0")]
    [Arguments("0a2")]
    [Arguments("0.A")]
    public async Task TryParseUtf8Int32_Invalid_False(string orig)
    {
        var buf = StringToBytes(orig);
        await Assert.That(buf.TryParseUtf8Int32(out _)).IsFalse();
    }

    [Test]
    [Arguments("")]
    [Arguments("-")]
    [Arguments(".")]
    [Arguments(".-0")]
    [Arguments("0a2")]
    [Arguments("0.A")]
    [Arguments("-1")]
    public async Task TryParseUtf8PositiveInt32_Invalid_False(string orig)
    {
        var buf = StringToBytes(orig);
        await Assert.That(buf.TryParseUtf8PositiveInt32(out _)).IsFalse();
    }

    [Test]
    [Arguments("")]
    [Arguments("-")]
    [Arguments(".")]
    [Arguments(".-0")]
    [Arguments("0a2")]
    [Arguments("0.A")]
    public async Task TryParseUtf8Int64_Invalid_False(string orig)
    {
        var buf = StringToBytes(orig);
        await Assert.That(buf.TryParseUtf8Int64(out _)).IsFalse();
    }

    [Test]
    [Arguments("")]
    [Arguments("-")]
    [Arguments(".")]
    [Arguments(".-0")]
    [Arguments("0a2")]
    [Arguments("0.A")]
    [Arguments("-1")]
    public async Task TryParseUtf8PositiveInt64_Invalid_False(string orig)
    {
        var buf = StringToBytes(orig);
        await Assert.That(buf.TryParseUtf8PositiveInt64(out _)).IsFalse();
    }

    [Test]
    [Arguments("")]
    [Arguments("-")]
    [Arguments(".")]
    [Arguments(".-0")]
    [Arguments("0a2")]
    [Arguments("0.A")]
    [Arguments("-1")]
    public async Task TryParseUtf8UInt32_Invalid_False(string orig)
    {
        var buf = StringToBytes(orig);
        await Assert.That(buf.TryParseUtf8UInt32(out _)).IsFalse();
    }

    [Test]
    [Arguments("")]
    [Arguments("-")]
    [Arguments(".")]
    [Arguments(".-0")]
    [Arguments("0a2")]
    [Arguments("0.A")]
    [Arguments("-1")]
    public async Task TryParseUtf8UInt64_Invalid_False(string orig)
    {
        var buf = StringToBytes(orig);
        await Assert.That(buf.TryParseUtf8UInt64(out _)).IsFalse();
    }

    [Test]
    public async Task TryParseUtf8Int32_Valid_True()
    {
        for (var x = 0; x < 100; ++x)
        {
            var num = RandomHelper.GetRandomInt32();
            var buf = StringToBytes(num.ToString());
            await Assert.That(buf.TryParseUtf8Int32(out var num2)).IsTrue();
            await Assert.That(num2).IsEqualTo(num);
        }
    }

    [Test]
    public async Task TryParseUtf8PositiveInt32_Valid_True()
    {
        for (var x = 0; x < 100; ++x)
        {
            var num = RandomHelper.GetRandomPositiveInt32();
            var buf = StringToBytes(num.ToString());
            await Assert.That(buf.TryParseUtf8PositiveInt32(out var num2)).IsTrue();
            await Assert.That(num2).IsEqualTo(num);
        }
    }

    [Test]
    public async Task TryParseUtf8Int64_Valid_True()
    {
        for (var x = 0; x < 100; ++x)
        {
            var num = RandomHelper.GetRandomInt64();
            var buf = StringToBytes(num.ToString());
            await Assert.That(buf.TryParseUtf8Int64(out var num2)).IsTrue();
            await Assert.That(num2).IsEqualTo(num);
        }
    }

    [Test]
    public async Task TryParseUtf8PositiveInt64_Valid_True()
    {
        for (var x = 0; x < 100; ++x)
        {
            var num = RandomHelper.GetRandomPositiveInt64();
            var buf = StringToBytes(num.ToString());
            await Assert.That(buf.TryParseUtf8PositiveInt64(out var num2)).IsTrue();
            await Assert.That(num2).IsEqualTo(num);
        }
    }

    [Test]
    public async Task TryParseUtf8UInt32_Valid_True()
    {
        for (var x = 0; x < 100; ++x)
        {
            var num = (uint)RandomHelper.GetRandomPositiveInt32();
            var buf = StringToBytes(num.ToString());
            await Assert.That(buf.TryParseUtf8UInt32(out var num2)).IsTrue();
            await Assert.That(num2).IsEqualTo(num);
        }
    }

    [Test]
    public async Task TryParseUtf8UInt64_Valid_True()
    {
        for (var x = 0; x < 100; ++x)
        {
            var num = (ulong)RandomHelper.GetRandomPositiveInt64();
            var buf = StringToBytes(num.ToString());
            await Assert.That(buf.TryParseUtf8UInt64(out var num2)).IsTrue();
            await Assert.That(num2).IsEqualTo(num);
        }
    }
}
