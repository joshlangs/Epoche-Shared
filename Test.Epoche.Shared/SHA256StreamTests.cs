using System.Security.Cryptography;

namespace Epoche.Shared;
public class SHA256StreamTests
{
    static MemoryStream GetStream(int length) => new(RandomHelper.GetRandomBytes(length));

    readonly SHA256 SHA256 = SHA256.Create();
    readonly MemoryStream MemoryStream = GetStream(1);
    readonly SHA256Stream SHA256Stream = new(GetStream(1));

    [Test]
    public async Task EmptyStream_HashZeroBytes()
    {
        using var s = GetStream(0);
        using var ss = new SHA256Stream(s);
        await Assert.That(ss.Read(new byte[1], 0, 1)).IsEqualTo(0);
        s.Position = 0;
        await Assert.That(SHA256.ComputeHash(s)).IsEquivalentTo(ss.SHA256Hash!);
    }

    [Test]
    public async Task ReadSpan_StreamHashesMatch()
    {
        using var s = GetStream(RandomHelper.GetRandomPositiveInt32() % 1000 + 1);
        var buf = new byte[s.Length];
        using var ss = new SHA256Stream(s);
        while (ss.Read(buf) > 0) { }
        s.Position = 0;
        await Assert.That(SHA256.ComputeHash(s)).IsEquivalentTo(ss.SHA256Hash!);
    }

    [Test]
    public async Task ReadWithLength_StreamHashesMatch()
    {
        using var s = GetStream(RandomHelper.GetRandomPositiveInt32() % 1000 + 1);
        var buf = new byte[s.Length];
        using var ss = new SHA256Stream(s);
        while (ss.Read(buf, 0, buf.Length) > 0) { }
        s.Position = 0;
        await Assert.That(SHA256.ComputeHash(s)).IsEquivalentTo(ss.SHA256Hash!);
    }

    [Test]
    public async Task AsyncReadWithLength_StreamHashesMatch()
    {
        using var s = GetStream(RandomHelper.GetRandomPositiveInt32() % 1000 + 1);
        var buf = new byte[s.Length];
        using var ss = new SHA256Stream(s);
        while (await ss.ReadAsync(buf.AsMemory(0, buf.Length), default) > 0) { }
        s.Position = 0;
        await Assert.That(SHA256.ComputeHash(s)).IsEquivalentTo(ss.SHA256Hash!);
    }

    [Test]
    public async Task AsyncReadMemory_StreamHashesMatch()
    {
        using var s = GetStream(RandomHelper.GetRandomPositiveInt32() % 1000 + 1);
        var buf = new byte[s.Length];
        using var ss = new SHA256Stream(s);
        while (await ss.ReadAsync(buf) > 0) { }
        s.Position = 0;
        await Assert.That(SHA256.ComputeHash(s)).IsEquivalentTo(ss.SHA256Hash!);
    }

    [Test]
    public async Task CanRead() => await Assert.That(SHA256Stream.CanRead).IsEqualTo(MemoryStream.CanRead);

    [Test]
    public async Task CanSeek() => await Assert.That(SHA256Stream.CanSeek).IsFalse();

    [Test]
    public async Task CanWrite() => await Assert.That(SHA256Stream.CanWrite).IsFalse();

    [Test]
    public async Task Length() => await Assert.That(SHA256Stream.Length).IsEqualTo(MemoryStream.Length);

    [Test]
    public async Task Position() => await Assert.That(SHA256Stream.Position).IsEqualTo(MemoryStream.Position);

    [Test]
    public async Task SetPosition() => Assert.Throws<NotSupportedException>(() => SHA256Stream.Position = 1);

    [Test]
    public async Task Seek() => Assert.Throws<NotSupportedException>(() => SHA256Stream.Seek(1, SeekOrigin.Begin));

    [Test]
    public async Task SetLength() => Assert.Throws<NotSupportedException>(() => SHA256Stream.SetLength(2));

    [Test]
    public async Task CanTimeout() => await Assert.That(SHA256Stream.CanTimeout).IsEqualTo(MemoryStream.CanTimeout);
}