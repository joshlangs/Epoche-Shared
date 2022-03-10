using System;
using System.Linq;
using Xunit;

namespace Epoche.Shared;

public class RandomHelperTests
{
    [Fact]
    [Trait("Type", "Unit")]
    public void GetRandomBytes_Negative_ThrowsArgumentOutOfRangeException() => Assert.Throws<ArgumentOutOfRangeException>(() => RandomHelper.GetRandomBytes(-1));

    [Fact]
    [Trait("Type", "Unit")]
    public void GetRandomBytes_Zero_ReturnsEmpty() => Assert.Empty(RandomHelper.GetRandomBytes(0));

    [Fact]
    [Trait("Type", "Unit")]
    public void GetRandomBytes_6_Returns6Bytes() => Assert.Equal(6, RandomHelper.GetRandomBytes(6).Length);

    [Fact]
    [Trait("Type", "Unit")]
    public void GetRandomBytes_BytesAreDifferent() => Assert.True(RandomHelper.GetRandomBytes(16).Distinct().Count() > 1);

    [Fact]
    [Trait("Type", "Unit")]
    public void GetRandomBytes_Null_ThrowsArgumentNullException() => Assert.Throws<ArgumentNullException>(() => RandomHelper.GetRandomBytes(null!));

    [Fact]
    [Trait("Type", "Unit")]
    public void GetRandomInt64_Repeated_IsDifferent() => Assert.True(Enumerable.Range(0, 4).Select(x => RandomHelper.GetRandomInt64()).Distinct().Count() > 1);

    [Fact]
    [Trait("Type", "Unit")]
    public void GetRandomInt32_Repeated_IsDifferent() => Assert.True(Enumerable.Range(0, 4).Select(x => RandomHelper.GetRandomInt32()).Distinct().Count() > 1);

    [Fact]
    [Trait("Type", "Unit")]
    public void GetRandomPositiveInt64_Repeated_IsDifferent() => Assert.True(Enumerable.Range(0, 4).Select(x => RandomHelper.GetRandomPositiveInt64()).Distinct().Count() > 1);

    [Fact]
    [Trait("Type", "Unit")]
    public void GetRandomPositiveInt32_Repeated_IsDifferent() => Assert.True(Enumerable.Range(0, 4).Select(x => RandomHelper.GetRandomPositiveInt32()).Distinct().Count() > 1);

    [Fact]
    [Trait("Type", "Unit")]
    public void GetRandomPositiveInt64_IsPositive() => Assert.True(RandomHelper.GetRandomPositiveInt64() > 0);

    [Fact]
    [Trait("Type", "Unit")]
    public void GetRandomPositiveInt32_IsPositive() => Assert.True(RandomHelper.GetRandomPositiveInt32() > 0);

    [Fact]
    [Trait("Type", "Unit")]
    public void GetRandomBytes_EmptyArray_DoesNotThrow() => RandomHelper.GetRandomBytes(Array.Empty<byte>());

    [Fact]
    [Trait("Type", "Unit")]
    public void GetRandomBytes_EmptySpan_DoesNotThrow() => RandomHelper.GetRandomBytes(Array.Empty<byte>().AsSpan());

    [Fact]
    [Trait("Type", "Unit")]
    public void GetRandomBytes_ByteArray_BytesAreDifferent()
    {
        var data = new byte[16];
        RandomHelper.GetRandomBytes(data);
        Assert.True(data.Distinct().Count() > 1);
    }

    [Fact]
    [Trait("Type", "Unit")]
    public void GetRandomBytes_ByteSpan_BytesAreDifferent()
    {
        var data = new byte[16];
        RandomHelper.GetRandomBytes(data.AsSpan());
        Assert.True(data.Distinct().Count() > 1);
    }

    [Fact]
    [Trait("Type", "Unit")]
    public void GetRandomLowerHex_Repeated_IsDifferent() => Assert.NotEqual(RandomHelper.GetRandomLowerHex(32), RandomHelper.GetRandomLowerHex(32));

    [Fact]
    [Trait("Type", "Unit")]
    public void GetRandomLowerHex_Length_Correct() => Assert.Equal(Enumerable.Range(0, 16).Select(x => RandomHelper.GetRandomLowerHex(x).Length), Enumerable.Range(0, 16));

    [Fact]
    [Trait("Type", "Unit")]
    public void GetRandomLowerHex_ReturnsHex()
    {
        for (var x = 0; x < 16; ++x)
        {
            var str = RandomHelper.GetRandomLowerHex(x);
            Assert.True(str.All(x => (x >= '0' && x <= '9') || (x >= 'a' && x <= 'f')));
        }
    }
}
