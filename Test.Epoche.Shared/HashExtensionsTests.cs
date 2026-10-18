using System.Security.Cryptography;

namespace Epoche.Shared;
public class HashExtensionsTests
{
    [Test]
    public async Task ComputeSHA256_BytesFromSHA256_Matches()
    {
        var b = RandomHelper.GetRandomBytes(32);
        using var hasher = SHA256.Create();
        var hash = hasher.ComputeHash(b);
        await Assert.That(b.ComputeSHA256()).IsEquivalentTo(hash);
    }

    [Test]
    public async Task ComputeSHA256_BytesWithOffsetFromSHA256_Matches()
    {
        var b = RandomHelper.GetRandomBytes(32);
        using var hasher = SHA256.Create();
        var hash = hasher.ComputeHash(b);
        await Assert.That(b.ComputeSHA256(0, b.Length)).IsEquivalentTo(hash);
    }

    [Test]
    public async Task ComputeSHA256_BytesSpanFromSHA256_Matches()
    {
        var b = RandomHelper.GetRandomBytes(32);
        using var hasher = SHA256.Create();
        var hash = hasher.ComputeHash(b);
        await Assert.That(b.AsSpan().ComputeSHA256()).IsEquivalentTo(hash);
    }

    [Test]
    public async Task ComputeSHA256_BytesReadOnlySpanFromSHA256_Matches()
    {
        var b = RandomHelper.GetRandomBytes(32);
        using var hasher = SHA256.Create();
        var hash = hasher.ComputeHash(b);
        await Assert.That(((ReadOnlySpan<byte>)b.AsSpan()).ComputeSHA256()).IsEquivalentTo(hash);
    }

    [Test]
    public async Task ComputeSHA256_SegmentFromSHA256_Matches()
    {
        var b = RandomHelper.GetRandomBytes(32);
        using var hasher = SHA256.Create();
        var hash = hasher.ComputeHash(b);
        await Assert.That(new ArraySegment<byte>(b).ComputeSHA256()).IsEquivalentTo(hash);
    }

    [Test]
    public async Task ComputeSHA256_SpanFromSHA256ToSpan_Matches()
    {
        var b = RandomHelper.GetRandomBytes(32);
        using var hasher = SHA256.Create();
        var hash = hasher.ComputeHash(b);
        Span<byte> dest = stackalloc byte[32];
        b.AsSpan().ComputeSHA256(dest);
        await Assert.That(dest.ToArray()).IsEquivalentTo(hash);
    }

    [Test]
    public async Task ComputeSHA256_ReadOnlySpanFromSHA256ToSpan_Matches()
    {
        var b = RandomHelper.GetRandomBytes(32);
        using var hasher = SHA256.Create();
        var hash = hasher.ComputeHash(b);
        Span<byte> dest = stackalloc byte[32];
        b.AsSpan().ComputeSHA256(dest);
        await Assert.That(((ReadOnlySpan<byte>)dest).ToArray()).IsEquivalentTo(hash);
    }
}
