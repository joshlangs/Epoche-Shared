using System.Linq;
using Xunit;

namespace Epoche.Shared;
public class ByteArrayEqualityComparerTests
{
    [Fact]
    [Trait("Type", "Unit")]
    public void GetHashCode_Null_Returns0() => Assert.Equal(0, ByteArrayEqualityComparer.Instance.GetHashCode(null));

    [Fact]
    [Trait("Type", "Unit")]
    public void GetHashCode_Empty_Returns0() => Assert.Equal(0, ByteArrayEqualityComparer.Instance.GetHashCode(new byte[0]));

    [Fact]
    [Trait("Type", "Unit")]
    public void GetHashCode_TwoInstances_DifferentCodes()
    {
        for (var x = 0; x < 128; ++x)
        {
            var buf1 = new byte[x];
            var buf2 = buf1.Concat(new byte[] { 0 }).ToArray();
            Assert.NotEqual(ByteArrayEqualityComparer.Instance.GetHashCode(buf1), ByteArrayEqualityComparer.Instance.GetHashCode(buf2));
            Assert.False(ByteArrayEqualityComparer.Instance.Equals(buf1, buf2));
        }
    }

    [Fact]
    [Trait("Type", "Unit")]
    public void GetHashCode_TwoInstances_EqualSameCodes()
    {
        for (var x = 0; x < 128; ++x)
        {
            var buf1 = RandomHelper.GetRandomBytes(x);
            var buf2 = buf1.ToArray();
            Assert.Equal(ByteArrayEqualityComparer.Instance.GetHashCode(buf1), ByteArrayEqualityComparer.Instance.GetHashCode(buf2));
            Assert.True(ByteArrayEqualityComparer.Instance.Equals(buf1, buf2));
        }
    }

    [Fact]
    [Trait("Type", "Unit")]
    public void Equals_EqualArrays_ReturnsTrue()
    {
        var bytes = RandomHelper.GetRandomBytes(32);
        Assert.True(ByteArrayEqualityComparer.Instance.Equals(bytes, bytes.ToArray()));
    }
    [Fact]
    [Trait("Type", "Unit")]
    public void Equals_UnequalLengthArrays_ReturnsFalse()
    {
        var bytes = RandomHelper.GetRandomBytes(32);
        Assert.False(ByteArrayEqualityComparer.Instance.Equals(bytes, bytes.Take(31).ToArray()));
    }
    [Fact]
    [Trait("Type", "Unit")]
    public void Equals_UnequalArrays_ReturnsFalse()
    {
        var bytes = RandomHelper.GetRandomBytes(32);
        var bytes2 = bytes.ToArray();
        bytes2[31]++;
        Assert.False(ByteArrayEqualityComparer.Instance.Equals(bytes, bytes2));
    }

    [Fact]
    [Trait("Type", "Unit")]
    public void Equals_TwoNulls_ReturnsTrue() => Assert.True(ByteArrayEqualityComparer.Instance.Equals(null, null));

    [Fact]
    [Trait("Type", "Unit")]
    public void Equals_LeftNull_ReturnsFalse() => Assert.False(ByteArrayEqualityComparer.Instance.Equals(null, new byte[1]));

    [Fact]
    [Trait("Type", "Unit")]
    public void Equals_RightNull_ReturnsFalse() => Assert.False(ByteArrayEqualityComparer.Instance.Equals(new byte[1], null));
}