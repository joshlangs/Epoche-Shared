using System;
using System.IO;
using System.Security.Cryptography;
using System.Threading.Tasks;
using Xunit;

namespace Epoche.Shared;
public class SHA256StreamTests
{
    static MemoryStream GetStream(int length) => new(RandomHelper.GetRandomBytes(length));

    readonly SHA256 SHA256 = SHA256.Create();
    readonly MemoryStream MemoryStream = GetStream(1);
    readonly SHA256Stream SHA256Stream = new(GetStream(1));

    [Fact]
    [Trait("Type", "Unit")]
    public void EmptyStream_HashZeroBytes()
    {
        using var s = GetStream(0);
        using var ss = new SHA256Stream(s);
        Assert.Equal(0, ss.Read(new byte[1], 0, 1));
        s.Position = 0;
        Assert.Equal(ss.SHA256Hash, SHA256.ComputeHash(s));
    }

    [Fact]
    [Trait("Type", "Unit")]
    public void ReadSpan_StreamHashesMatch()
    {
        using var s = GetStream(RandomHelper.GetRandomPositiveInt32() % 1000 + 1);
        var buf = new byte[s.Length];
        using var ss = new SHA256Stream(s);
        while (ss.Read(buf) > 0) { }
        s.Position = 0;
        Assert.Equal(ss.SHA256Hash, SHA256.ComputeHash(s));
    }

    [Fact]
    [Trait("Type", "Unit")]
    public void ReadWithLength_StreamHashesMatch()
    {
        using var s = GetStream(RandomHelper.GetRandomPositiveInt32() % 1000 + 1);
        var buf = new byte[s.Length];
        using var ss = new SHA256Stream(s);
        while (ss.Read(buf, 0, buf.Length) > 0) { }
        s.Position = 0;
        Assert.Equal(ss.SHA256Hash, SHA256.ComputeHash(s));
    }

    [Fact]
    [Trait("Type", "Unit")]
    public async Task AsyncReadWithLength_StreamHashesMatch()
    {
        using var s = GetStream(RandomHelper.GetRandomPositiveInt32() % 1000 + 1);
        var buf = new byte[s.Length];
        using var ss = new SHA256Stream(s);
        while (await ss.ReadAsync(buf, 0, buf.Length, default) > 0) { }
        s.Position = 0;
        Assert.Equal(ss.SHA256Hash, SHA256.ComputeHash(s));
    }

    [Fact]
    [Trait("Type", "Unit")]
    public async Task AsyncReadMemory_StreamHashesMatch()
    {
        using var s = GetStream(RandomHelper.GetRandomPositiveInt32() % 1000 + 1);
        var buf = new byte[s.Length];
        using var ss = new SHA256Stream(s);
        while (await ss.ReadAsync(buf) > 0) { }
        s.Position = 0;
        Assert.Equal(ss.SHA256Hash, SHA256.ComputeHash(s));
    }

    [Fact]
    [Trait("Type", "Unit")]
    public void CanRead() => Assert.Equal(MemoryStream.CanRead, SHA256Stream.CanRead);

    [Fact]
    [Trait("Type", "Unit")]
    public void CanSeek() => Assert.False(SHA256Stream.CanSeek);

    [Fact]
    [Trait("Type", "Unit")]
    public void CanWrite() => Assert.False(SHA256Stream.CanWrite);

    [Fact]
    [Trait("Type", "Unit")]
    public void Length() => Assert.Equal(MemoryStream.Length, SHA256Stream.Length);

    [Fact]
    [Trait("Type", "Unit")]
    public void Position() => Assert.Equal(MemoryStream.Position, SHA256Stream.Position);

    [Fact]
    [Trait("Type", "Unit")]
    public void SetPosition() => Assert.Throws<NotSupportedException>(() => SHA256Stream.Position = 1);

    [Fact]
    [Trait("Type", "Unit")]
    public void Seek() => Assert.Throws<NotSupportedException>(() => SHA256Stream.Seek(1, SeekOrigin.Begin));

    [Fact]
    [Trait("Type", "Unit")]
    public void SetLength() => Assert.Throws<NotSupportedException>(() => SHA256Stream.SetLength(2));

    [Fact]
    [Trait("Type", "Unit")]
    public void CanTimeout() => Assert.Equal(MemoryStream.CanTimeout, SHA256Stream.CanTimeout);
}