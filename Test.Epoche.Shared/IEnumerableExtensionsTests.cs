using System.Numerics;

namespace Epoche.Shared;

public class IEnumerableExtensionsTests
{
    [Test]
    public async Task ReuseableSegment_0MaxItems_ThrowsArgumentOutOfRangeExceptionWhenEnumerated() => Assert.Throws<ArgumentOutOfRangeException>(() => Array.Empty<int>().ReuseableSegment(0).ToList());

    [Test]
    public async Task ReuseableSegment_NegativeMaxItems_ThrowsArgumentOutOfRangeExceptionWhenEnumerated() => Assert.Throws<ArgumentOutOfRangeException>(() => Array.Empty<int>().ReuseableSegment(-1).ToList());

    [Test]
    public async Task ReuseableSegment_NullItems_ReturnsEmpty() => await Assert.That(IEnumerableExtensions.ReuseableSegment<int>(null, 1)).IsEmpty();

    [Test]
    public async Task ReuseableSegment_EmptyItems_ReturnsEmpty() => await Assert.That(IEnumerableExtensions.ReuseableSegment(Array.Empty<int>(), 1)).IsEmpty();

    [Test]
    public async Task ReuseableSegment_4Items1PerSegment_Returns4Segments() => await Assert.That(new int[4].ReuseableSegment(1).Count()).IsEqualTo(4);

    [Test]
    public async Task ReuseableSegment_4Items2PerSegment_Returns2Segments() => await Assert.That(new int[4].ReuseableSegment(2).Count()).IsEqualTo(2);
    [Test]
    public async Task ReuseableSegment_4Items3PerSegment_Returns2Segments() => await Assert.That(new int[4].ReuseableSegment(3).Count()).IsEqualTo(2);

    [Test]
    public async Task ReuseableSegment_4Items4PerSegment_Returns1Segments() => await Assert.That(new int[4].ReuseableSegment(4)).HasSingleItem();

    [Test]
    public async Task ReuseableSegment_4Items5PerSegment_Returns1Segments() => await Assert.That(new int[4].ReuseableSegment(5)).HasSingleItem();

    [Test]
    public async Task ReuseableSegment_Items_FormSameCollection()
    {
        var rnd = new Random();
        var items = Enumerable.Range(0, 5 + rnd.Next() % 100).Select(x => rnd.Next()).ToArray();
        await Assert.That(items.ReuseableSegment(1 + rnd.Next() % 10).SelectMany(x => x)).IsEquivalentTo(items);
    }

    [Test]
    public async Task ReuseableSegment_Items_SegmentsNotReused()
    {
        var segments = new int[2].ReuseableSegment(1).ToList();
        await Assert.That(segments[0]).IsNotSameReferenceAs(segments[1]);
    }


    [Test]
    public async Task Segment_0MaxItems_ThrowsArgumentOutOfRangeExceptionWhenEnumerated() => Assert.Throws<ArgumentOutOfRangeException>(() => Array.Empty<int>().Segment(0).ToList());

    [Test]
    public async Task Segment_NegativeMaxItems_ThrowsArgumentOutOfRangeExceptionWhenEnumerated() => Assert.Throws<ArgumentOutOfRangeException>(() => Array.Empty<int>().Segment(-1).ToList());

    [Test]
    public async Task Segment_NullItems_ReturnsEmpty() => await Assert.That(IEnumerableExtensions.Segment<int>(null, 1)).IsEmpty();

    [Test]
    public async Task Segment_EmptyItems_ReturnsEmpty() => await Assert.That(IEnumerableExtensions.Segment(Array.Empty<int>(), 1)).IsEmpty();

    [Test]
    public async Task Segment_4Items1PerSegment_Returns4Segments() => await Assert.That(new int[4].Segment(1).Select(x => x.Count()).Count()).IsEqualTo(4);

    [Test]
    public async Task Segment_4Items2PerSegment_Returns2Segments() => await Assert.That(new int[4].Segment(2).Select(x => x.Count()).Count()).IsEqualTo(2);
    [Test]
    public async Task Segment_4Items3PerSegment_Returns2Segments() => await Assert.That(new int[4].Segment(3).Select(x => x.Count()).Count()).IsEqualTo(2);

    [Test]
    public async Task Segment_4Items4PerSegment_Returns1Segments() => await Assert.That(new int[4].Segment(4).Select(x => x.Count())).HasSingleItem();

    [Test]
    public async Task Segment_4Items5PerSegment_Returns1Segments() => await Assert.That(new int[4].Segment(5).Select(x => x.Count())).HasSingleItem();

    [Test]
    public async Task Segment_Items_FormSameCollection()
    {
        var rnd = new Random();
        var items = Enumerable.Range(0, 5 + rnd.Next() % 100).Select(x => rnd.Next()).ToArray();
        await Assert.That(items.Segment(1 + rnd.Next() % 10).Select(x => x.ToList()).SelectMany(x => x)).IsEquivalentTo(items);
    }

    [Test]
    public async Task Segment_1Items1PerSegmentNotEnumerated_ReturnsInfiniteSegments() => await Assert.That(new int[1].Segment(1).Take(100).Count()).IsEqualTo(100);

    [Test]
    public async Task EmptyIfNull_Null_ReturnsEmpty() => await Assert.That(((IEnumerable<int>?)null).EmptyIfNull()).IsEmpty();

    [Test]
    public async Task EmptyIfNull_NonNull_ReturnsOriginal()
    {
        var b = new byte[2];
        await Assert.That(b.EmptyIfNull()).IsSameReferenceAs(b);
    }

    [Test]
    public async Task ExcludeNull_NoNulls_ReturnsOriginal()
    {
        var objs = new[] { new object(), new object() };
        await Assert.That(objs.ExcludeNull()).IsEquivalentTo(objs);
    }

    [Test]
    public async Task ExcludeNull_OnlyNulls_ReturnsEmpty()
    {
        var objs = new object?[] { null, null };
        await Assert.That(objs.ExcludeNull()).IsEmpty();
    }

    [Test]
    public async Task ExcludeNull_SomeNulls_ReturnsNonNulls()
    {
        var objs = new object?[] { new object(), null, new object(), null };
        await Assert.That(objs.ExcludeNull()).IsEquivalentTo(objs.Where(x => x is not null));
    }

    [Test]
    public async Task SumBigInteger_Null_Throws()
    {
        Assert.Throws<ArgumentNullException>(() => ((BigInteger[])null!).SumBigInteger());
    }

    [Test]
    public async Task SumBigInteger_Empty_0()
    {
        await Assert.That(Array.Empty<BigInteger>().SumBigInteger()).IsEqualTo(BigInteger.Zero);
    }

    [Test]
    public async Task SumBigInteger_One_4()
    {
        await Assert.That(new BigInteger[] { 4 }.SumBigInteger()).IsEqualTo((BigInteger)4);
    }

    [Test]
    public async Task SumBigInteger_Multi_Correct()
    {
        await Assert.That(new BigInteger[] { 4, -6, 7 }.SumBigInteger()).IsEqualTo((BigInteger)5);
    }

}
