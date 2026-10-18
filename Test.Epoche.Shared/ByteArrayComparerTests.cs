namespace Epoche.Shared;

public class ByteArrayComparerTests
{
    [Test]
    public async Task NullEqualsNull() => await Assert.That(ByteArrayComparer.Instance.Compare(null, null)).IsEqualTo(0);

    [Test]
    public async Task NotNullGreaterThanNull()
    {
        await Assert.That(ByteArrayComparer.Instance.Compare([], null)).IsEqualTo(-1);
        await Assert.That(ByteArrayComparer.Instance.Compare(null, [])).IsEqualTo(1);
    }

    [Test]
    public async Task EmptyEqualsEmpty() => await Assert.That(ByteArrayComparer.Instance.Compare([], [])).IsEqualTo(0);

    [Test]
    public async Task SameEqualsSame() => await Assert.That(ByteArrayComparer.Instance.Compare([1, 2, 3], [1, 2, 3])).IsEqualTo(0);

    [Test]
    public async Task LongerGreaterThanShorter()
    {
        await Assert.That(ByteArrayComparer.Instance.Compare([1, 2, 3], [1, 2, 3, 4])).IsEqualTo(-1);
        await Assert.That(ByteArrayComparer.Instance.Compare([1, 2, 3, 4], [1, 2, 3])).IsEqualTo(1);
    }

    [Test]
    public async Task SortedByteOrder()
    {
        await Assert.That(ByteArrayComparer.Instance.Compare([0, 2, 3, 4], [1, 2, 3])).IsEqualTo(-1);
        await Assert.That(ByteArrayComparer.Instance.Compare([1, 2, 3], [0, 2, 3, 4])).IsEqualTo(1);

        await Assert.That(ByteArrayComparer.Instance.Compare([1, 2, 3, 4], [1, 2, 3])).IsEqualTo(1);
        await Assert.That(ByteArrayComparer.Instance.Compare([1, 2, 3], [1, 2, 3, 4])).IsEqualTo(-1);
    }
}
