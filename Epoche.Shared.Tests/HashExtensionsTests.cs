using System;
using System.Security.Cryptography;
using Xunit;

namespace Epoche.Shared;
public class HashExtensionsTests
{
    [Fact]
    [Trait("Type", "Unit")]
    public void ComputeSHA256_BytesFromSHA256_Matches()
    {
        var b = RandomHelper.GetRandomBytes(32);
        using var hasher = SHA256.Create();
        var hash = hasher.ComputeHash(b);
        Assert.Equal(hash, b.ComputeSHA256());
    }

    [Fact]
    [Trait("Type", "Unit")]
    public void ComputeSHA256_BytesWithOffsetFromSHA256_Matches()
    {
        var b = RandomHelper.GetRandomBytes(32);
        using var hasher = SHA256.Create();
        var hash = hasher.ComputeHash(b);
        Assert.Equal(hash, b.ComputeSHA256(0, b.Length));
    }

    [Fact]
    [Trait("Type", "Unit")]
    public void ComputeSHA256_BytesSpanFromSHA256_Matches()
    {
        var b = RandomHelper.GetRandomBytes(32);
        using var hasher = SHA256.Create();
        var hash = hasher.ComputeHash(b);
        Assert.Equal(hash, b.AsSpan().ComputeSHA256());
    }

    [Fact]
    [Trait("Type", "Unit")]
    public void ComputeSHA256_BytesReadOnlySpanFromSHA256_Matches()
    {
        var b = RandomHelper.GetRandomBytes(32);
        using var hasher = SHA256.Create();
        var hash = hasher.ComputeHash(b);
        Assert.Equal(hash, ((ReadOnlySpan<byte>)b.AsSpan()).ComputeSHA256());
    }

    [Fact]
    [Trait("Type", "Unit")]
    public void ComputeSHA256_SegmentFromSHA256_Matches()
    {
        var b = RandomHelper.GetRandomBytes(32);
        using var hasher = SHA256.Create();
        var hash = hasher.ComputeHash(b);
        Assert.Equal(hash, new ArraySegment<byte>(b).ComputeSHA256());
    }

    [Fact]
    [Trait("Type", "Unit")]
    public void ComputeSHA256_SpanFromSHA256ToSpan_Matches()
    {
        var b = RandomHelper.GetRandomBytes(32);
        using var hasher = SHA256.Create();
        var hash = hasher.ComputeHash(b);
        Span<byte> dest = stackalloc byte[32];
        b.AsSpan().ComputeSHA256(dest);
        Assert.Equal(hash, dest.ToArray());
    }

    [Fact]
    [Trait("Type", "Unit")]
    public void ComputeSHA256_ReadOnlySpanFromSHA256ToSpan_Matches()
    {
        var b = RandomHelper.GetRandomBytes(32);
        using var hasher = SHA256.Create();
        var hash = hasher.ComputeHash(b);
        Span<byte> dest = stackalloc byte[32];
        b.AsSpan().ComputeSHA256(dest);
        Assert.Equal(hash, ((ReadOnlySpan<byte>)dest).ToArray());
    }
}
