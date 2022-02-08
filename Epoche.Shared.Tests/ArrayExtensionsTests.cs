using System;
using System.Linq;
using Xunit;

namespace Epoche.Shared;
public class ArrayExtensionsTests
{
    [Fact]
    [Trait("Type", "Unit")]
    public void ReverseArrayInPlace_Array_ReturnsSameObjectReference()
    {
        var b = RandomHelper.GetRandomBytes(32);
        var b2 = b.ReverseArrayInPlace();
        Assert.Same(b, b2);
    }

    [Fact]
    [Trait("Type", "Unit")]
    public void ReverseArrayInPlace_Array_ReturnsReversedBytes()
    {
        var b = RandomHelper.GetRandomBytes(32);
        var b2 = b.ToArray();
        Assert.Equal(b.ReverseArrayInPlace(), b2.Reverse().ToArray());
    }

    [Fact]
    [Trait("Type", "Unit")]
    public void ReverseArrayInPlace_Null_ThrowsArgumentNullException() => Assert.Throws<ArgumentNullException>(() => ((byte[])null!).ReverseArrayInPlace());

    [Fact]
    [Trait("Type", "Unit")]
    public void ConcatArray_LeftNull_ThrowsArgumentNullException() => Assert.Throws<ArgumentNullException>(() => ((byte[])null!).ConcatArray(new byte[1]));

    [Fact]
    [Trait("Type", "Unit")]
    public void ConcatArray_RightNull_ThrowsArgumentNullException() => Assert.Throws<ArgumentNullException>(() => new byte[1].ConcatArray(null!));

    [Fact]
    [Trait("Type", "Unit")]
    public void ConcatArray_BothNull_ThrowsArgumentNullException() => Assert.Throws<ArgumentNullException>(() => ((byte[])null!).ConcatArray(null!));

    [Fact]
    [Trait("Type", "Unit")]
    public void ConcatArray_TwoArrays_AreConcatenated()
    {
        var a = RandomHelper.GetRandomBytes(32);
        var b = RandomHelper.GetRandomBytes(16);
        Assert.Equal(a.Concat(b), a.ConcatArray(b));
    }
}