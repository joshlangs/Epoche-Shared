using System.Buffers;
using System.Globalization;

namespace Epoche.Shared;

public class StringExtensionsTests
{
    static byte[] CrappyToBytes(string str)
    {
        var result = new byte[str.Length / 2];
        for (var x = 0; x < result.Length; ++x)
        {
            result[x] = byte.Parse(str.Substring(x * 2, 2), NumberStyles.HexNumber);
        }
        return result;
    }

    [Test]
    public async Task ToLowerHex_Null_ThrowsArgumentNullException() => Assert.Throws<ArgumentNullException>(() => ((byte[])null!).ToLowerHex());

    [Test]
    public async Task ToLowerHex_AllBytes_ReturnHexString()
    {
        for (var x = 0; x < 256; ++x)
        {
            await Assert.That(x.ToString("x2")).IsEqualTo(new byte[] { (byte)x }.ToLowerHex());
        }
    }

    [Test]
    public async Task ToLowerHex_AllBytesSpan_ReturnHexString()
    {
        for (var x = 0; x < 256; ++x)
        {
            await Assert.That(x.ToString("x2")).IsEqualTo(new byte[] { (byte)x }.AsSpan().ToLowerHex());
        }
    }

    [Test]
    public async Task ToLowerHex_LargeSpan_RoundTrips()
    {
        var data = RandomHelper.GetRandomBytes(1000000);
        var str = data.AsSpan().ToLowerHex();
        await Assert.That(data).IsEquivalentTo(str.ToHexBytes());
    }

    [Test]
    public async Task ToLowerHex_RandomBytes_ReturnHexString()
    {
        var buf = RandomHelper.GetRandomBytes(32);
        await Assert.That(string.Concat(buf.Select(x => x.ToString("x2")))).IsEqualTo(buf.ToLowerHex());
    }

    [Test]
    public async Task ToLowerHex_RandomBytesMemory_ReturnHexString()
    {
        var buf = RandomHelper.GetRandomBytes(32);
        await Assert.That(string.Concat(buf.Select(x => x.ToString("x2")))).IsEqualTo(buf.AsMemory().ToLowerHex());
    }

    [Test]
    public async Task ToLowerHex_RandomBytesSpan_ReturnHexString()
    {
        var buf = RandomHelper.GetRandomBytes(32);
        await Assert.That(string.Concat(buf.Select(x => x.ToString("x2")))).IsEqualTo(buf.AsSpan().ToLowerHex());
    }

    [Test]
    public async Task TryToBigInteger_Null_ReturnsNull() => Assert.Null(((string)null!).TryToBigInteger());
    [Test]
    public async Task TryToBigFraction_Null_ReturnsNull() => Assert.Null(((string)null!).TryToBigFraction());
    [Test]
    public async Task TryToInt64_Null_ReturnsNull() => Assert.Null(((string)null!).TryToInt64());
    [Test]
    public async Task TryToInt32_Null_ReturnsNull() => Assert.Null(((string)null!).TryToInt32());
    [Test]
    public async Task TryToDouble_Null_ReturnsNull() => Assert.Null(((string)null!).TryToDouble());
    [Test]
    public async Task TryToDecimal_Null_ReturnsNull() => Assert.Null(((string)null!).TryToDecimal());


    [Test]
    public async Task TryToBigInteger_InvalidNumber_ReturnsNull() => Assert.Null("A".TryToBigInteger());
    [Test]
    public async Task TryToBigFraction_InvalidNumber_ReturnsNull() => Assert.Null("A".TryToBigFraction());
    [Test]
    public async Task TryToInt64_InvalidNumber_ReturnsNull() => Assert.Null("A".TryToInt64());
    [Test]
    public async Task TryToInt32_InvalidNumber_ReturnsNull() => Assert.Null("A".TryToInt32());
    [Test]
    public async Task TryToDouble_InvalidNumber_ReturnsNull() => Assert.Null("A".TryToDouble());
    [Test]
    public async Task TryToDecimal_InvalidNumber_ReturnsNull() => Assert.Null("A".TryToDecimal());

    [Test]
    public async Task TryToBigInteger_Returns_CorrectValue() => await Assert.That("1234".TryToBigInteger().ToString()).IsEqualTo("1234");
    [Test]
    public async Task TryToBigFraction_Returns_CorrectValue() => await Assert.That("12.34".TryToBigFraction().ToString()).IsEqualTo("12.34");
    [Test]
    public async Task TryToInt64_Returns_CorrectValue() => await Assert.That("1234".TryToInt64().ToString()).IsEqualTo("1234");
    [Test]
    public async Task TryToInt32_Returns_CorrectValue() => await Assert.That("1234".TryToInt32().ToString()).IsEqualTo("1234");
    [Test]
    public async Task TryToDouble_Returns_CorrectValue() => await Assert.That("123.4".TryToDouble().ToString()).IsEqualTo("123.4");
    [Test]
    public async Task TryToDecimal_Returns_CorrectValue() => await Assert.That("123.4".TryToDecimal().ToString()).IsEqualTo("123.4");

    [Test]
    public async Task ToHexBytes_Null_ThrowsArgumentNullException() => Assert.Throws<ArgumentNullException>(() => ((string)null!).ToHexBytes());

    [Test]
    public async Task ToHexBytes_OddLengthString_ThrowsFormatException() => Assert.Throws<FormatException>(() => "123".ToHexBytes());

    [Test]
    [Arguments("z1")]
    [Arguments("1z")]
    [Arguments(" 0")]
    public async Task ToHexBytes_BadString_ThrowsFormatException(string str) => Assert.Throws<FormatException>(() => str.ToHexBytes());

    [Test]
    public async Task ToHexBytes_AllBytes_ReturnCorrectByte()
    {
        for (var x = 0; x < 256; ++x)
        {
            await Assert.That(x.ToString("x2").ToHexBytes()).IsEquivalentTo([(byte)x]);
        }
    }

    [Test]
    public async Task TryToHexBytes_Null_ReturnsNull() => Assert.Null(((string)null!).TryToHexBytes());

    [Test]
    public async Task TryToHexBytes_OddLengthString_ReturnsNull() => Assert.Null("123".TryToHexBytes());

    [Test]
    [Arguments("z1")]
    [Arguments("1z")]
    [Arguments(" 0")]
    public async Task TryToHexBytes_BadString_ReturnsNull(string str) => Assert.Null(str.TryToHexBytes());

    [Test]
    public async Task TryToHexBytes_AllBytes_ReturnCorrectByte()
    {
        for (var x = 0; x < 256; ++x)
        {
            await Assert.That(x.ToString("x2").TryToHexBytes()!).IsEquivalentTo([(byte)x]);
        }
    }

    [Test]
    public async Task ToHexBytes_SpanNull_ThrowsArgumentNullException() => Assert.Throws<ArgumentNullException>(() => ((string)null!).ToHexBytes([]));

    [Test]
    public async Task ToHexBytes_SpanOddLengthString_ThrowsArgumentOutOfRangeException() => Assert.Throws<ArgumentOutOfRangeException>(() => "123".ToHexBytes(new byte[2]));

    [Test]
    [Arguments("z1")]
    [Arguments("1z")]
    [Arguments(" 0")]
    public async Task ToHexBytes_SpanBadString_ThrowsArgumentOutOfRangeException(string str) => Assert.Throws<ArgumentOutOfRangeException>(() => str.ToHexBytes(new byte[1]));

    [Test]
    public async Task ToHexBytes_SpanAllBytes_ReturnCorrectByte()
    {
        var data = new byte[1];
        for (var x = 0; x < 256; ++x)
        {
            x.ToString("x2").ToHexBytes(data);
            await Assert.That(data[0]).IsEqualTo((byte)x);
        }
    }

    [Test]
    public async Task NullIfEmpty_Null_ReturnsNull() => Assert.Null(((string)null!).NullIfEmpty());

    [Test]
    public async Task NullIfEmpty_Empty_ReturnsNull() => Assert.Null("".NullIfEmpty());

    [Test]
    public async Task NullIfEmpty_Whitespace_ReturnsString() => await Assert.That(" ".NullIfEmpty()).IsEqualTo(" ");

    [Test]
    public async Task NullIfEmpty_String_ReturnsString() => await Assert.That("xxx".NullIfEmpty()).IsEqualTo("xxx");

    [Test]
    public async Task ToLowerHexUtf8_AllBytesSpan_SetCorrectBytes()
    {
        for (var x = 0; x < 256; ++x)
        {
            var target = new byte[2];
            new byte[] { (byte)x }.AsSpan().ToLowerHexUtf8(target.AsSpan());
            await Assert.That(x.ToString("x2")).IsEqualTo(new string([.. target.Select(x => (char)x)]));
        }
    }
    [Test]
    public async Task ToLowerHexUtf8_AllBytesReadOnlySpan_SetCorrectBytes()
    {
        for (var x = 0; x < 256; ++x)
        {
            var target = new byte[2];
            ((ReadOnlySpan<byte>)[(byte)x]).ToLowerHexUtf8(target.AsSpan());
            await Assert.That(x.ToString("x2")).IsEqualTo(new string([.. target.Select(x => (char)x)]));
        }
    }

    [Test]
    public async Task ToLowerHexUtf8_AllBytesSpan_SetCorrectChars()
    {
        for (var x = 0; x < 256; ++x)
        {
            var target = new char[2];
            new byte[] { (byte)x }.AsSpan().ToLowerHexUtf8(target.AsSpan());
            await Assert.That(x.ToString("x2")).IsEqualTo(new string(target));
        }
    }

    [Test]
    public async Task ToLowerHexUtf8_AllBytesReadOnlySpan_SetCorrectChars()
    {
        for (var x = 0; x < 256; ++x)
        {
            var target = new char[2];
            ((ReadOnlySpan<byte>)[(byte)x]).ToLowerHexUtf8(target.AsSpan());
            await Assert.That(x.ToString("x2")).IsEqualTo(new string(target));
        }
    }

    [Test]
    [Arguments(0, 1)]
    [Arguments(0, 2)]
    [Arguments(1, 0)]
    [Arguments(1, 1)]
    [Arguments(1, 3)]
    [Arguments(1, 4)]
    [Arguments(10, 10)]
    [Arguments(10, 19)]
    [Arguments(10, 21)]
    public async Task ToLowerHexUtf8_WrongSizeBytes_Throws(int byteSize, int bufferSize) =>
        Assert.Throws<ArgumentOutOfRangeException>(() => new byte[byteSize].AsSpan().ToLowerHexUtf8(new byte[bufferSize]));

    [Test]
    [Arguments(0, 1)]
    [Arguments(0, 2)]
    [Arguments(1, 0)]
    [Arguments(1, 1)]
    [Arguments(1, 3)]
    [Arguments(1, 4)]
    [Arguments(10, 10)]
    [Arguments(10, 19)]
    [Arguments(10, 21)]
    public async Task ToLowerHexUtf8_WrongSizeChars_Throws(int byteSize, int bufferSize) =>
        Assert.Throws<ArgumentOutOfRangeException>(() => new byte[byteSize].AsSpan().ToLowerHexUtf8(new char[bufferSize]));


    [Test]
    public async Task ToLowerHexUtf8_ZeroBytes_DoesntThrow() => Array.Empty<byte>().AsSpan().ToLowerHexUtf8(Array.Empty<byte>());

    [Test]
    public async Task ToLowerHexUtf8_ZeroChars_DoesntThrow() => Array.Empty<byte>().AsSpan().ToLowerHexUtf8(Array.Empty<char>());

    [Test]
    public async Task ToLowerHexUtf8_RandomBytes_MatchesToLowerHex()
    {
        var data = RandomHelper.GetRandomBytes(RandomHelper.GetRandomPositiveInt32() % 1024);
        var buf = new byte[data.Length * 2];
        data.AsSpan().ToLowerHexUtf8(buf);
        await Assert.That(data.ToLowerHex()).IsEqualTo(new string([.. buf.Select(x => (char)x)]));
    }

    [Test]
    public async Task ToLowerHexUtf8_RandomChars_MatchesToLowerHex()
    {
        var data = RandomHelper.GetRandomBytes(RandomHelper.GetRandomPositiveInt32() % 1024);
        var buf = new char[data.Length * 2];
        data.AsSpan().ToLowerHexUtf8(buf);
        await Assert.That(data.ToLowerHex()).IsEqualTo(new string(buf));
    }

    [Test]
    public async Task ToBytesFromHexUtf8_AllBytesSpan_SetsCorrectBytes()
    {
        for (var x = 0; x < 256; ++x)
        {
            var data = new byte[] { (byte)x }.ToLowerHex().Select(x => (byte)x).ToArray();
            var target = new byte[1];
            data.AsSpan().ToBytesFromHexUtf8(target);
            await Assert.That(target[0]).IsEqualTo((byte)x);
        }
    }

    [Test]
    public async Task ToBytesFromHexUtf8_AllBytesReadOnlySpan_SetsCorrectBytes()
    {
        for (var x = 0; x < 256; ++x)
        {
            var data = new byte[] { (byte)x }.ToLowerHex().Select(x => (byte)x).ToArray();
            var target = new byte[1];
            ((ReadOnlySpan<byte>)data.AsSpan()).ToBytesFromHexUtf8(target);
            await Assert.That(target[0]).IsEqualTo((byte)x);
        }
    }

    [Test]
    public async Task ToBytesFromHexUtf8_AllCharsSpan_SetsCorrectBytes()
    {
        for (var x = 0; x < 256; ++x)
        {
            var data = new byte[] { (byte)x }.ToLowerHex().Select(x => (byte)x).ToArray();
            var target = new char[1];
            data.AsSpan().ToBytesFromHexUtf8(target);
            await Assert.That(target[0]).IsEqualTo((char)x);
        }
    }

    [Test]
    public async Task ToBytesFromHexUtf8_AllCharsReadOnlySpan_SetsCorrectBytes()
    {
        for (var x = 0; x < 256; ++x)
        {
            var data = new byte[] { (byte)x }.ToLowerHex().Select(x => (byte)x).ToArray();
            var target = new char[1];
            ((ReadOnlySpan<byte>)data.AsSpan()).ToBytesFromHexUtf8(target);
            await Assert.That(target[0]).IsEqualTo((char)x);
        }
    }

    [Test]
    public async Task ToBytesFromHexUtf8_AllBytesUppercaseSpan_SetsCorrectBytes()
    {
        for (var x = 0; x < 256; ++x)
        {
            var data = new byte[] { (byte)x }.ToLowerHex().ToUpper().Select(x => (byte)x).ToArray();
            var target = new byte[1];
            data.AsSpan().ToBytesFromHexUtf8(target);
            await Assert.That(target[0]).IsEqualTo((byte)x);
        }
    }

    [Test]
    public async Task ToBytesFromHexUtf8_AllBytesUppercaseReadOnlySpan_SetsCorrectBytes()
    {
        for (var x = 0; x < 256; ++x)
        {
            var data = new byte[] { (byte)x }.ToLowerHex().ToUpper().Select(x => (byte)x).ToArray();
            var target = new byte[1];
            ((ReadOnlySpan<byte>)data.AsSpan()).ToBytesFromHexUtf8(target);
            await Assert.That(target[0]).IsEqualTo((byte)x);
        }
    }

    [Test]
    public async Task ToBytesFromHexUtf8_AllCharsUppercaseSpan_SetsCorrectBytes()
    {
        for (var x = 0; x < 256; ++x)
        {
            var data = new byte[] { (byte)x }.ToLowerHex().ToUpper().Select(x => (byte)x).ToArray();
            var target = new char[1];
            data.AsSpan().ToBytesFromHexUtf8(target);
            await Assert.That(target[0]).IsEqualTo((char)x);
        }
    }

    [Test]
    public async Task ToBytesFromHexUtf8_AllCharsUppercaseReadOnlySpan_SetsCorrectBytes()
    {
        for (var x = 0; x < 256; ++x)
        {
            var data = new byte[] { (byte)x }.ToLowerHex().ToUpper().Select(x => (byte)x).ToArray();
            var target = new char[1];
            ((ReadOnlySpan<byte>)data.AsSpan()).ToBytesFromHexUtf8(target);
            await Assert.That(target[0]).IsEqualTo((char)x);
        }
    }

    [Test]
    public async Task ToBytesFromHexUtf8_ZeroBytesSpan_DoesntThrow() => Array.Empty<byte>().AsSpan().ToBytesFromHexUtf8(Array.Empty<byte>());

    [Test]
    public async Task ToBytesFromHexUtf8_ZeroCharsSpan_DoesntThrow() => Array.Empty<byte>().AsSpan().ToBytesFromHexUtf8(Array.Empty<char>());

    [Test]
    public async Task ToBytesFromHexUtf8_ZeroBytesReadOnlySpan_DoesntThrow() => ((ReadOnlySpan<byte>)[]).ToBytesFromHexUtf8(Array.Empty<byte>());

    [Test]
    public async Task ToBytesFromHexUtf8_ZeroCharsReadOnlySpan_DoesntThrow() => ((ReadOnlySpan<byte>)[]).ToBytesFromHexUtf8(Array.Empty<char>());

    [Test]
    [Arguments(0, 1)]
    [Arguments(0, 2)]
    [Arguments(1, 0)]
    [Arguments(1, 1)]
    [Arguments(1, 3)]
    [Arguments(1, 4)]
    [Arguments(10, 10)]
    [Arguments(10, 19)]
    [Arguments(10, 21)]
    public async Task ToBytesFromHexUtf8_WrongSizeBytesSpan_Throws(int bufferSize, int byteSize) =>
        Assert.Throws<ArgumentOutOfRangeException>(() => new byte[byteSize].AsSpan().ToBytesFromHexUtf8(new byte[bufferSize]));

    [Test]
    [Arguments(0, 1)]
    [Arguments(0, 2)]
    [Arguments(1, 0)]
    [Arguments(1, 1)]
    [Arguments(1, 3)]
    [Arguments(1, 4)]
    [Arguments(10, 10)]
    [Arguments(10, 19)]
    [Arguments(10, 21)]
    public async Task ToBytesFromHexUtf8_WrongSizeBytesReadOnlySpan_Throws(int bufferSize, int byteSize) =>
        Assert.Throws<ArgumentOutOfRangeException>(() => ((ReadOnlySpan<byte>)new byte[byteSize].AsSpan()).ToBytesFromHexUtf8(new byte[bufferSize]));

    [Test]
    [Arguments(0, 1)]
    [Arguments(0, 2)]
    [Arguments(1, 0)]
    [Arguments(1, 1)]
    [Arguments(1, 3)]
    [Arguments(1, 4)]
    [Arguments(10, 10)]
    [Arguments(10, 19)]
    [Arguments(10, 21)]
    public async Task ToBytesFromHexUtf8_WrongSizeCharsSpan_Throws(int bufferSize, int byteSize) =>
        Assert.Throws<ArgumentOutOfRangeException>(() => new byte[byteSize].AsSpan().ToBytesFromHexUtf8(new char[bufferSize]));

    [Test]
    [Arguments(0, 1)]
    [Arguments(0, 2)]
    [Arguments(1, 0)]
    [Arguments(1, 1)]
    [Arguments(1, 3)]
    [Arguments(1, 4)]
    [Arguments(10, 10)]
    [Arguments(10, 19)]
    [Arguments(10, 21)]
    public async Task ToBytesFromHexUtf8_WrongSizeCharsReadOnlySpan_Throws(int bufferSize, int byteSize) =>
        Assert.Throws<ArgumentOutOfRangeException>(() => ((ReadOnlySpan<byte>)new byte[byteSize].AsSpan()).ToBytesFromHexUtf8(new char[bufferSize]));

    [Test]
    public async Task ToBytesFromHexUtf8_InvalidHexBytesSpan_Throws()
    {
        for (var x = 0; x < 256; ++x)
        {
            if (x >= '0' && x <= '9')
            {
                continue;
            }
            if (x >= 'a' && x <= 'f')
            {
                continue;
            }
            if (x >= 'A' && x <= 'F')
            {
                continue;
            }

            Assert.Throws<FormatException>(() => new byte[] { (byte)0, (byte)x }.AsSpan().ToBytesFromHexUtf8(new byte[1]));
            Assert.Throws<FormatException>(() => new byte[] { (byte)x, (byte)x }.AsSpan().ToBytesFromHexUtf8(new byte[1]));
            Assert.Throws<FormatException>(() => new byte[] { (byte)x, (byte)0 }.AsSpan().ToBytesFromHexUtf8(new byte[1]));
        }
    }

    [Test]
    public async Task ToBytesFromHexUtf8_InvalidHexBytesReadOnlySpan_Throws()
    {
        for (var x = 0; x < 256; ++x)
        {
            if (x >= '0' && x <= '9')
            {
                continue;
            }
            if (x >= 'a' && x <= 'f')
            {
                continue;
            }
            if (x >= 'A' && x <= 'F')
            {
                continue;
            }

            Assert.Throws<FormatException>(() => ((ReadOnlySpan<byte>)[(byte)0, (byte)x]).ToBytesFromHexUtf8(new byte[1]));
            Assert.Throws<FormatException>(() => ((ReadOnlySpan<byte>)[(byte)x, (byte)x]).ToBytesFromHexUtf8(new byte[1]));
            Assert.Throws<FormatException>(() => ((ReadOnlySpan<byte>)[(byte)x, (byte)0]).ToBytesFromHexUtf8(new byte[1]));
        }
    }

    [Test]
    public async Task ToBytesFromHexUtf8_InvalidHexCharsSpan_Throws()
    {
        for (var x = 0; x < 256; ++x)
        {
            if (x >= '0' && x <= '9')
            {
                continue;
            }
            if (x >= 'a' && x <= 'f')
            {
                continue;
            }
            if (x >= 'A' && x <= 'F')
            {
                continue;
            }

            Assert.Throws<FormatException>(() => new byte[] { (byte)0, (byte)x }.AsSpan().ToBytesFromHexUtf8(new char[1]));
            Assert.Throws<FormatException>(() => new byte[] { (byte)x, (byte)x }.AsSpan().ToBytesFromHexUtf8(new char[1]));
            Assert.Throws<FormatException>(() => new byte[] { (byte)x, (byte)0 }.AsSpan().ToBytesFromHexUtf8(new char[1]));
        }
    }

    [Test]
    public async Task ToBytesFromHexUtf8_InvalidHexCharsReadOnlySpan_Throws()
    {
        for (var x = 0; x < 256; ++x)
        {
            if (x >= '0' && x <= '9')
            {
                continue;
            }
            if (x >= 'a' && x <= 'f')
            {
                continue;
            }
            if (x >= 'A' && x <= 'F')
            {
                continue;
            }

            Assert.Throws<FormatException>(() => ((ReadOnlySpan<byte>)[(byte)0, (byte)x]).ToBytesFromHexUtf8(new char[1]));
            Assert.Throws<FormatException>(() => ((ReadOnlySpan<byte>)[(byte)x, (byte)x]).ToBytesFromHexUtf8(new char[1]));
            Assert.Throws<FormatException>(() => ((ReadOnlySpan<byte>)[(byte)x, (byte)0]).ToBytesFromHexUtf8(new char[1]));
        }
    }

    [Test]
    public async Task ToBytesFromHexUtf8_EmptyBytesSequence_DoesntThrow() => ReadOnlySequence<byte>.Empty.ToBytesFromHexUtf8([]);
    [Test]
    public async Task ToBytesFromHexUtf8_EmptyCharsSequence_DoesntThrow() => ReadOnlySequence<char>.Empty.ToBytesFromHexUtf8([]);

    [Test]
    public async Task ToBytesFromHexUtf8_AllByteSequences_SingleContiguous_SetsCorrectBytes()
    {
        for (var x = 0; x < 256; ++x)
        {
            var target = new byte[1];
            new ReadOnlySequence<byte>([.. new[] { (byte)x }.ToLowerHex().Select(x => (byte)x)]).ToBytesFromHexUtf8(target);
            await Assert.That(x).IsEqualTo(target[0]);
        }
    }

    [Test]
    public async Task ToBytesFromHexUtf8_AllCharsSequences_SingleContiguous_SetsCorrectBytes()
    {
        for (var x = 0; x < 256; ++x)
        {
            var target = new byte[1];
            new ReadOnlySequence<char>([.. new[] { (byte)x }.ToLowerHex()]).ToBytesFromHexUtf8(target);
            await Assert.That(x).IsEqualTo(target[0]);
        }
    }

    [Test]
    public async Task ToBytesFromHexUtf8_AllByteSequences_SingleContiguousUppercase_SetsCorrectBytes()
    {
        for (var x = 0; x < 256; ++x)
        {
            var target = new byte[1];
            new ReadOnlySequence<byte>([.. new[] { (byte)x }.ToLowerHex().ToUpper().Select(x => (byte)x)]).ToBytesFromHexUtf8(target);
            await Assert.That(x).IsEqualTo(target[0]);
        }
    }

    [Test]
    public async Task ToBytesFromHexUtf8_AllCharsSequences_SingleContiguousUppercase_SetsCorrectBytes()
    {
        for (var x = 0; x < 256; ++x)
        {
            var target = new byte[1];
            new ReadOnlySequence<char>([.. new[] { (byte)x }.ToLowerHex().ToUpper()]).ToBytesFromHexUtf8(target);
            await Assert.That(x).IsEqualTo(target[0]);
        }
    }

    class ByteSeq : ReadOnlySequenceSegment<byte>
    {
        public ByteSeq(ByteSeq prev, byte data) : this(prev, [data]) { }
        public ByteSeq(ByteSeq prev, byte[] data)
        {
            if (prev is not null)
            {
                prev.Next = this;
                RunningIndex = prev.RunningIndex + prev.Memory.Length;
            }
            Memory = data;
        }
    }
    class CharSeq : ReadOnlySequenceSegment<char>
    {
        public CharSeq(CharSeq prev, char data) : this(prev, [data]) { }
        public CharSeq(CharSeq prev, char[] data)
        {
            if (prev is not null)
            {
                prev.Next = this;
                RunningIndex = prev.RunningIndex + prev.Memory.Length;
            }
            Memory = data;
        }
    }

    [Test]
    public async Task ToBytesFromHexUtf8_AllByteSequences_SingleSplit_SetsCorrectBytes()
    {
        for (var x = 0; x < 256; ++x)
        {
            var target = new byte[1];
            var hex = new[] { (byte)x }.ToLowerHex().Select(x => (byte)x).ToArray();
            var seq1 = new ByteSeq(null!, hex[0]);
            var seq2 = new ByteSeq(seq1, hex[1]);
            var seq = new ReadOnlySequence<byte>(seq1, 0, seq2, 1);
            seq.ToBytesFromHexUtf8(target);
            await Assert.That(x).IsEqualTo(target[0]);
        }
    }

    [Test]
    public async Task ToBytesFromHexUtf8_AllCharSequences_SingleSplit_SetsCorrectBytes()
    {
        for (var x = 0; x < 256; ++x)
        {
            var target = new byte[1];
            var hex = new[] { (byte)x }.ToLowerHex().ToArray();
            var seq1 = new CharSeq(null!, hex[0]);
            var seq2 = new CharSeq(seq1, hex[1]);
            var seq = new ReadOnlySequence<char>(seq1, 0, seq2, 1);
            seq.ToBytesFromHexUtf8(target);
            await Assert.That(x).IsEqualTo(target[0]);
        }
    }

    [Test]
    public async Task ToBytesFromHexUtf8_InvalidHexBytesSequence_Throws()
    {
        for (var x = 0; x < 256; ++x)
        {
            if (x >= '0' && x <= '9')
            {
                continue;
            }
            if (x >= 'a' && x <= 'f')
            {
                continue;
            }
            if (x >= 'A' && x <= 'F')
            {
                continue;
            }

            Assert.Throws<FormatException>(() => new ReadOnlySequence<byte>([(byte)0, (byte)x]).ToBytesFromHexUtf8(new byte[1]));
            Assert.Throws<FormatException>(() => new ReadOnlySequence<byte>([(byte)x, (byte)x]).ToBytesFromHexUtf8(new byte[1]));
            Assert.Throws<FormatException>(() => new ReadOnlySequence<byte>([(byte)x, (byte)0]).ToBytesFromHexUtf8(new byte[1]));
        }
    }

    [Test]
    public async Task ToBytesFromHexUtf8_InvalidHexCharsSequence_Throws()
    {
        for (var x = 0; x < 256; ++x)
        {
            if (x >= '0' && x <= '9')
            {
                continue;
            }
            if (x >= 'a' && x <= 'f')
            {
                continue;
            }
            if (x >= 'A' && x <= 'F')
            {
                continue;
            }

            Assert.Throws<FormatException>(() => new ReadOnlySequence<char>([.. new byte[] { (byte)0, (byte)x }.Select(x => (char)x)]).ToBytesFromHexUtf8(new byte[1]));
            Assert.Throws<FormatException>(() => new ReadOnlySequence<char>([.. new byte[] { (byte)x, (byte)x }.Select(x => (char)x)]).ToBytesFromHexUtf8(new byte[1]));
            Assert.Throws<FormatException>(() => new ReadOnlySequence<char>([.. new byte[] { (byte)x, (byte)0 }.Select(x => (char)x)]).ToBytesFromHexUtf8(new byte[1]));
        }
    }

    [Test]
    public async Task ToBytesFromHexUtf8_RandomBytesSingle_MatchesToHexBytes()
    {
        var hexstr = RandomHelper.GetRandomBytes(RandomHelper.GetRandomPositiveInt32() % 1024).ToLowerHex();
        var buf = new byte[hexstr.Length / 2];
        new ReadOnlySequence<byte>([.. hexstr.Select(x => (byte)x)]).ToBytesFromHexUtf8(buf);
        await Assert.That(hexstr.ToHexBytes()).IsEquivalentTo(buf);
    }

    [Test]
    public async Task ToBytesFromHexUtf8_RandonCharsSingle_MatchesToHexBytes()
    {
        var hexstr = RandomHelper.GetRandomBytes(RandomHelper.GetRandomPositiveInt32() % 1024).ToLowerHex();
        var buf = new byte[hexstr.Length / 2];
        new ReadOnlySequence<char>([.. hexstr]).ToBytesFromHexUtf8(buf);
        await Assert.That(hexstr.ToHexBytes()).IsEquivalentTo(buf);
    }

    [Test]
    public async Task ToBytesFromHexUtf8_RandomBytesSplit_MatchesToHexBytes()
    {
        var hexstr = RandomHelper.GetRandomBytes(2 + RandomHelper.GetRandomPositiveInt32() % 1024).ToLowerHex();
        var buf = new byte[hexstr.Length / 2];
        var split = hexstr.Length / 2 + RandomHelper.GetRandomInt32() % 2;
        var seq1 = new ByteSeq(null!, [.. hexstr[..split].Select(x => (byte)x)]);
        var seq2 = new ByteSeq(seq1, [.. hexstr[split..].Select(x => (byte)x)]);
        new ReadOnlySequence<byte>(seq1, 0, seq2, seq2.Memory.Length).ToBytesFromHexUtf8(buf);
        await Assert.That(hexstr.ToHexBytes()).IsEquivalentTo(buf);
    }

    [Test]
    public async Task ToBytesFromHexUtf8_RandomCharsSplit_MatchesToHexBytes()
    {
        var hexstr = RandomHelper.GetRandomBytes(2 + RandomHelper.GetRandomPositiveInt32() % 1024).ToLowerHex();
        var buf = new byte[hexstr.Length / 2];
        var split = hexstr.Length / 2 + RandomHelper.GetRandomInt32() % 2;
        var seq1 = new CharSeq(null!, [.. hexstr[..split]]);
        var seq2 = new CharSeq(seq1, [.. hexstr[split..]]);
        new ReadOnlySequence<char>(seq1, 0, seq2, seq2.Memory.Length).ToBytesFromHexUtf8(buf);
        await Assert.That(hexstr.ToHexBytes()).IsEquivalentTo(buf);
    }

    [Test]
    [Arguments("")]
    [Arguments("03")]
    [Arguments("3f")]
    [Arguments("f83eb1")]
    [Arguments("ffff0000abcd7632")]
    public async Task ToLowerHex_ByteSpan_TestCases(string str) => await Assert.That(CrappyToBytes(str).AsSpan().ToLowerHex()).IsEqualTo(str);

    [Test]
    [Arguments("")]
    [Arguments("03")]
    [Arguments("3f")]
    [Arguments("f83eb1")]
    [Arguments("ffff0000abcd7632")]
    public async Task ToLowerHex_ByteReadOnlySpan_TestCases(string str) => await Assert.That(((ReadOnlySpan<byte>)CrappyToBytes(str).AsSpan()).ToLowerHex()).IsEqualTo(str);

    [Test]
    [Arguments("")]
    [Arguments("03")]
    [Arguments("3f")]
    [Arguments("f83eb1")]
    [Arguments("ffff0000abcd7632")]
    public async Task ToBytesFromHexUtf8_ByteSpan_TestCases(string str)
    {
        var expected = CrappyToBytes(str);
        var data = new byte[expected.Length];
        str.Select(x => (byte)x).ToArray().AsSpan().ToBytesFromHexUtf8(data);
        await Assert.That(data).IsEquivalentTo(expected);
    }

    [Test]
    [Arguments("")]
    [Arguments("03")]
    [Arguments("3f")]
    [Arguments("f83eb1")]
    [Arguments("ffff0000abcd7632")]
    public async Task ToBytesFromHexUtf8_ByteReadOnlySpan_TestCases(string str)
    {
        var expected = CrappyToBytes(str);
        var data = new byte[expected.Length];
        ((ReadOnlySpan<byte>)[.. str.Select(x => (byte)x)]).ToBytesFromHexUtf8(data);
        await Assert.That(data).IsEquivalentTo(expected);
    }

    [Test]
    [Arguments("")]
    [Arguments("03")]
    [Arguments("3f")]
    [Arguments("f83eb1")]
    [Arguments("ffff0000abcd7632")]
    public async Task ToBytesFromHexUtf8_CharSpan_TestCases(string str)
    {
        var expected = CrappyToBytes(str);
        var data = new char[expected.Length];
        str.Select(x => (byte)x).ToArray().AsSpan().ToBytesFromHexUtf8(data);
        await Assert.That(data).IsEquivalentTo(expected.Select(x => (char)x));
    }

    [Test]
    [Arguments("")]
    [Arguments("03")]
    [Arguments("3f")]
    [Arguments("f83eb1")]
    [Arguments("ffff0000abcd7632")]
    public async Task ToBytesFromHexUtf8_CharReadOnlySpan_TestCases(string str)
    {
        var expected = CrappyToBytes(str);
        var data = new char[expected.Length];
        ((ReadOnlySpan<byte>)[.. str.Select(x => (byte)x)]).ToBytesFromHexUtf8(data);
        await Assert.That(data).IsEquivalentTo(expected.Select(x => (char)x));
    }

    [Test]
    public async Task Truncate_Negative_Throws() => Assert.Throws<ArgumentOutOfRangeException>(() => "abc".Truncate(-1));

    [Test]
    [Arguments("", 0, "")]
    [Arguments("", 1, "")]
    [Arguments("a", 0, "")]
    [Arguments("a", 1, "a")]
    [Arguments("a", 2, "a")]
    [Arguments("test", int.MaxValue, "test")]
    [Arguments("test", 2, "te")]
    public async Task Truncate_Length_TestCases(string input, int maxLength, string expected) => await Assert.That(input.Truncate(maxLength)).IsEqualTo(expected);

    [Test]
    public async Task Truncate_WithStart_NegativeStart_Throws() => Assert.Throws<ArgumentOutOfRangeException>(() => "abc".Truncate(-1, 0));

    [Test]
    public async Task Truncate_WithStart_NegativeLength_Throws() => Assert.Throws<ArgumentOutOfRangeException>(() => "abc".Truncate(0, -1));

    [Test]
    public async Task Truncate_WithStart_StartingIndexTooHigh_Throws() => Assert.Throws<ArgumentOutOfRangeException>(() => "abc".Truncate(4, 0));

    [Test]
    [Arguments("", 0, 0, "")]
    [Arguments("", 0, 1, "")]
    [Arguments("a", 0, 0, "")]
    [Arguments("a", 0, 1, "a")]
    [Arguments("a", 1, 0, "")]
    [Arguments("a", 1, int.MaxValue, "")]
    [Arguments("abc", 2, int.MaxValue, "c")]
    [Arguments("abc", 0, 3, "abc")]
    [Arguments("abc", 1, 1, "b")]
    [Arguments("abc", 1, 2, "bc")]
    [Arguments("abc", 1, 4, "bc")]
    public async Task Truncate_WithStart_TestCases(string input, int startingIndex, int maxLength, string expected) => await Assert.That(input.Truncate(startingIndex, maxLength)).IsEqualTo(expected);

    [Test]
    [Arguments("")]
    [Arguments("12")]
    [Arguments("ab1234")]
    [Arguments("abcdef1234567890")]
    [Arguments("AB1234")]
    [Arguments("abcdef1234567890ABCDEF")]
    public async Task IsHexBytes_Valid_ReturnsTrue(string value) => await Assert.That(value.IsHexBytes()).IsTrue();

    [Test]
    [Arguments("1")]
    [Arguments("123")]
    [Arguments("gb1234")]
    [Arguments("aGcdef1234567890")]
    public async Task IsHexBytes_Invalid_ReturnsFalse(string value) => await Assert.That(value.IsHexBytes()).IsFalse();
}
