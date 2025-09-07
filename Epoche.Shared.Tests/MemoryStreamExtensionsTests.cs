using System;
using System.IO;
using System.Linq;
using Xunit;

namespace Epoche.Shared;
public class MemoryStreamExtensionsTests
{
    [Fact]
    [Trait("Type", "Unit")]
    public void GetDataBuffer_Null_ThrowsArgumentNullException() => Assert.Throws<ArgumentNullException>(() => MemoryStreamExtensions.GetDataBuffer(null!));

    [Fact]
    [Trait("Type", "Unit")]
    public void GetDataBuffer_Empty_ReturnsEmpty() => Assert.Empty(new MemoryStream().GetDataBuffer().ToList());

    [Fact]
    [Trait("Type", "Unit")]
    public void GetDataBuffer_NonwritableStream_ReturnsSameBytes()
    {
        var bytes = RandomHelper.GetRandomBytes(RandomHelper.GetRandomPositiveInt32() % 1000 + 1);
        Assert.Equal(bytes, new MemoryStream(bytes, false).GetDataBuffer().AsSpan());
    }

    [Fact]
    [Trait("Type", "Unit")]
    public void GetDataBuffer_WritableStream_ReturnsSameBytes()
    {
        var bytes = RandomHelper.GetRandomBytes(RandomHelper.GetRandomPositiveInt32() % 1000 + 1);
        var ms = new MemoryStream();
        ms.Write(bytes);
        Assert.Equal(bytes, ms.GetDataBuffer().AsSpan());
    }

    [Fact]
    [Trait("Type", "Unit")]
    public void GetDataSpan_Null_ThrowsArgumentNullException() => Assert.Throws<ArgumentNullException>(() => MemoryStreamExtensions.GetDataSpan(null!));

    [Fact]
    [Trait("Type", "Unit")]
    public void GetDataSpan_Empty_ReturnsEmpty() => Assert.Empty(new MemoryStream().GetDataSpan().ToArray());


    [Fact]
    [Trait("Type", "Unit")]
    public void GetDataSpan_NonwritableStream_ReturnsSameBytes()
    {
        var bytes = RandomHelper.GetRandomBytes(RandomHelper.GetRandomPositiveInt32() % 1000 + 1);
        Assert.Equal(bytes, new MemoryStream(bytes, false).GetDataSpan().ToArray());
    }

    [Fact]
    [Trait("Type", "Unit")]
    public void GetDataSpan_WritableStream_ReturnsSameBytes()
    {
        var bytes = RandomHelper.GetRandomBytes(RandomHelper.GetRandomPositiveInt32() % 1000 + 1);
        var ms = new MemoryStream();
        ms.Write(bytes);
        Assert.Equal(bytes, ms.GetDataSpan().ToArray());
    }

}