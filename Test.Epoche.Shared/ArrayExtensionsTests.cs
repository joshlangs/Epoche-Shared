namespace Epoche.Shared;
public class ArrayExtensionsTests
{
    [Test]
    public async Task ReverseArrayInPlace_Array_ReturnsSameObjectReference()
    {
        var b = RandomHelper.GetRandomBytes(32);
        var b2 = b.ReverseArrayInPlace();
        await Assert.That(b).IsSameReferenceAs(b2);
    }

    [Test]
    public async Task ReverseArrayInPlace_Array_ReturnsReversedBytes()
    {
        var b = RandomHelper.GetRandomBytes(32);
        var b2 = b.Reverse().ToArray();
        await Assert.That(b2).IsEquivalentTo(b.ReverseArrayInPlace());
    }

    [Test]
    public async Task ReverseArrayInPlace_Null_ThrowsArgumentNullException() => Assert.Throws<ArgumentNullException>(() => ((byte[])null!).ReverseArrayInPlace());

    [Test]
    public async Task ConcatArray_LeftNull_ThrowsArgumentNullException() => Assert.Throws<ArgumentNullException>(() => ((byte[])null!).ConcatArray(new byte[1]));

    [Test]
    public async Task ConcatArray_RightNull_ThrowsArgumentNullException() => Assert.Throws<ArgumentNullException>(() => new byte[1].ConcatArray(null!));

    [Test]
    public async Task ConcatArray_BothNull_ThrowsArgumentNullException() => Assert.Throws<ArgumentNullException>(() => ((byte[])null!).ConcatArray(null!));

    [Test]
    public async Task ConcatArray_TwoArrays_AreConcatenated()
    {
        var a = RandomHelper.GetRandomBytes(32);
        var b = RandomHelper.GetRandomBytes(16);
        await Assert.That(a.ConcatArray(b)).IsEquivalentTo(a.Concat(b));
    }
}