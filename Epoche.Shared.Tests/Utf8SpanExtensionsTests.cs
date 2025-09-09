using System;
using Xunit;

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

    [Fact]
    public void TryToAsciiCharSpan_Empty_True() => Assert.True(((ReadOnlySpan<byte>)[]).TryToAsciiCharSpan([]));

    [Fact]
    public void TryToAsciiCharSpan_TooShort_False() => Assert.False(((ReadOnlySpan<byte>)new byte[1].AsSpan()).TryToAsciiCharSpan([]));

    [Fact]
    public void TryToAsciiCharSpan_32_126_True()
    {
        var buf = new byte[126 - 32 + 1];
        for (var x = 0; x < buf.Length; ++x)
        {
            buf[x] = (byte)(32 + x);
        }

        var chars = new char[buf.Length];
        Assert.True(((ReadOnlySpan<byte>)buf.AsSpan()).TryToAsciiCharSpan(chars));
        for (var x = 0; x < buf.Length; ++x)
        {
            Assert.Equal(chars[x], (char)buf[x]);
        }
    }

    [Fact]
    public void TryToAsciiCharSpan_NotAscii_False()
    {
        var buf = new byte[1];
        var r = (ReadOnlySpan<byte>)buf.AsSpan();
        var chars = new char[1];
        for (var x = 0; x < 32; ++x)
        {
            buf[0] = (byte)x;
            Assert.False(r.TryToAsciiCharSpan(chars));
        }
        for (var x = 127; x <= byte.MaxValue; ++x)
        {
            buf[0] = (byte)x;
            Assert.False(r.TryToAsciiCharSpan(chars));
        }
    }

    [Theory]
    [InlineData("")]
    [InlineData("-")]
    [InlineData(".")]
    [InlineData(".-0")]
    [InlineData("0a2")]
    [InlineData("0.A")]
    public void TryParseUtf8Int32_Invalid_False(string orig)
    {
        var buf = StringToBytes(orig);
        Assert.False(buf.TryParseUtf8Int32(out _));
    }

    [Theory]
    [InlineData("")]
    [InlineData("-")]
    [InlineData(".")]
    [InlineData(".-0")]
    [InlineData("0a2")]
    [InlineData("0.A")]
    [InlineData("-1")]
    public void TryParseUtf8PositiveInt32_Invalid_False(string orig)
    {
        var buf = StringToBytes(orig);
        Assert.False(buf.TryParseUtf8PositiveInt32(out _));
    }

    [Theory]
    [InlineData("")]
    [InlineData("-")]
    [InlineData(".")]
    [InlineData(".-0")]
    [InlineData("0a2")]
    [InlineData("0.A")]
    public void TryParseUtf8Int64_Invalid_False(string orig)
    {
        var buf = StringToBytes(orig);
        Assert.False(buf.TryParseUtf8Int64(out _));
    }

    [Theory]
    [InlineData("")]
    [InlineData("-")]
    [InlineData(".")]
    [InlineData(".-0")]
    [InlineData("0a2")]
    [InlineData("0.A")]
    [InlineData("-1")]
    public void TryParseUtf8PositiveInt64_Invalid_False(string orig)
    {
        var buf = StringToBytes(orig);
        Assert.False(buf.TryParseUtf8PositiveInt64(out _));
    }

    [Theory]
    [InlineData("")]
    [InlineData("-")]
    [InlineData(".")]
    [InlineData(".-0")]
    [InlineData("0a2")]
    [InlineData("0.A")]
    [InlineData("-1")]
    public void TryParseUtf8UInt32_Invalid_False(string orig)
    {
        var buf = StringToBytes(orig);
        Assert.False(buf.TryParseUtf8UInt32(out _));
    }

    [Theory]
    [InlineData("")]
    [InlineData("-")]
    [InlineData(".")]
    [InlineData(".-0")]
    [InlineData("0a2")]
    [InlineData("0.A")]
    [InlineData("-1")]
    public void TryParseUtf8UInt64_Invalid_False(string orig)
    {
        var buf = StringToBytes(orig);
        Assert.False(buf.TryParseUtf8UInt64(out _));
    }

    [Fact]
    public void TryParseUtf8Int32_Valid_True()
    {
        for (var x = 0; x < 100; ++x)
        {
            var num = RandomHelper.GetRandomInt32();
            var buf = StringToBytes(num.ToString());
            Assert.True(buf.TryParseUtf8Int32(out var num2));
            Assert.Equal(num, num2);
        }
    }

    [Fact]
    public void TryParseUtf8PositiveInt32_Valid_True()
    {
        for (var x = 0; x < 100; ++x)
        {
            var num = RandomHelper.GetRandomPositiveInt32();
            var buf = StringToBytes(num.ToString());
            Assert.True(buf.TryParseUtf8PositiveInt32(out var num2));
            Assert.Equal(num, num2);
        }
    }

    [Fact]
    public void TryParseUtf8Int64_Valid_True()
    {
        for (var x = 0; x < 100; ++x)
        {
            var num = RandomHelper.GetRandomInt64();
            var buf = StringToBytes(num.ToString());
            Assert.True(buf.TryParseUtf8Int64(out var num2));
            Assert.Equal(num, num2);
        }
    }

    [Fact]
    public void TryParseUtf8PositiveInt64_Valid_True()
    {
        for (var x = 0; x < 100; ++x)
        {
            var num = RandomHelper.GetRandomPositiveInt64();
            var buf = StringToBytes(num.ToString());
            Assert.True(buf.TryParseUtf8PositiveInt64(out var num2));
            Assert.Equal(num, num2);
        }
    }

    [Fact]
    public void TryParseUtf8UInt32_Valid_True()
    {
        for (var x = 0; x < 100; ++x)
        {
            var num = (uint)RandomHelper.GetRandomPositiveInt32();
            var buf = StringToBytes(num.ToString());
            Assert.True(buf.TryParseUtf8UInt32(out var num2));
            Assert.Equal(num, num2);
        }
    }

    [Fact]
    public void TryParseUtf8UInt64_Valid_True()
    {
        for (var x = 0; x < 100; ++x)
        {
            var num = (ulong)RandomHelper.GetRandomPositiveInt64();
            var buf = StringToBytes(num.ToString());
            Assert.True(buf.TryParseUtf8UInt64(out var num2));
            Assert.Equal(num, num2);
        }
    }
}
