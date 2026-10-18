namespace Epoche.Shared;
public class IListExtensionsTests_Sorted
{
    readonly List<int> Items = new();

    readonly List<int> ItemsZero = new() { 0 };
    readonly List<int> ItemsOne = new() { 1 };
    readonly List<int> TenEven = [.. Enumerable.Range(0, 10).Select(x => x * 2)];

    readonly Random Random = new();

    [Test]
    public async Task BinarySearch_ItemsNull_ThrowsArgumentNullException() => Assert.Throws<ArgumentNullException>(() => IListExtensions.BinarySearch(null!, 0, Comparer<int>.Default));
    [Test]
    public async Task BinarySearch_ItemNull_ThrowsArgumentNullException() => Assert.Throws<ArgumentNullException>(() => IListExtensions.BinarySearch(new List<object>(), null!, Comparer<object>.Default));
    [Test]
    public async Task BinarySearch_ComparerNull_ThrowsArgumentNullException() => Assert.Throws<ArgumentNullException>(() => IListExtensions.BinarySearch(Items, 0, null!));

    [Test]
    public async Task SortedInsert_ItemsNull_ThrowsArgumentNullException() => Assert.Throws<ArgumentNullException>(() => IListExtensions.SortedInsert(null!, 0, Comparer<int>.Default));
    [Test]
    public async Task SortedInsert_ItemNull_ThrowsArgumentNullException() => Assert.Throws<ArgumentNullException>(() => IListExtensions.SortedInsert(new List<object>(), null!, Comparer<object>.Default));
    [Test]
    public async Task SortedInsert_ComparerNull_ThrowsArgumentNullException() => Assert.Throws<ArgumentNullException>(() => IListExtensions.SortedInsert(Items, 0, null!));

    [Test]
    public async Task SortedDelete_ItemsNull_ThrowsArgumentNullException() => Assert.Throws<ArgumentNullException>(() => IListExtensions.SortedDelete(null!, 0, Comparer<int>.Default));
    [Test]
    public async Task SortedDelete_ItemNull_ThrowsArgumentNullException() => Assert.Throws<ArgumentNullException>(() => IListExtensions.SortedDelete(new List<object>(), null!, Comparer<object>.Default));
    [Test]
    public async Task SortedDelete_ComparerNull_ThrowsArgumentNullException() => Assert.Throws<ArgumentNullException>(() => IListExtensions.SortedDelete(Items, 0, null!));


    [Test]
    public async Task BinarySearch_Empty_ReturnsCmp0() => await Assert.That(IListExtensions.BinarySearch(Items, 0, Comparer<int>.Default)).IsEqualTo(~0);

    [Test]
    public async Task BinarySearch_Match_Returns0() => await Assert.That(IListExtensions.BinarySearch(ItemsZero, 0, Comparer<int>.Default)).IsEqualTo(0);

    [Test]
    public async Task BinarySearch_NoMatch_Greater_ReturnsCmp1() => await Assert.That(IListExtensions.BinarySearch(ItemsZero, 1, Comparer<int>.Default)).IsEqualTo(~1);

    [Test]
    public async Task BinarySearch_NoMatch_Lesser_ReturnsCmp0() => await Assert.That(IListExtensions.BinarySearch(ItemsOne, 0, Comparer<int>.Default)).IsEqualTo(~0);

    [Test]
    public async Task BinarySearch_AllFound()
    {
        for (var x = 0; x < TenEven.Count; ++x)
        {
            await Assert.That(IListExtensions.BinarySearch(TenEven, TenEven[x], Comparer<int>.Default)).IsEqualTo(x);
        }
    }
    [Test]
    public async Task BinarySearch_NoneFound_Lesser()
    {
        for (var x = 0; x < TenEven.Count; ++x)
        {
            await Assert.That(IListExtensions.BinarySearch(TenEven, TenEven[x] - 1, Comparer<int>.Default)).IsEqualTo(~x);
        }
    }
    [Test]
    public async Task BinarySearch_NoneFound_Greater()
    {
        for (var x = 0; x < TenEven.Count; ++x)
        {
            await Assert.That(IListExtensions.BinarySearch(TenEven, TenEven[x] + 1, Comparer<int>.Default)).IsEqualTo(~(x + 1));
        }
    }

    [Test]
    public async Task Delete_Empty_ReturnsFalse() => await Assert.That(IListExtensions.SortedDelete(Items, 0, Comparer<int>.Default)).IsFalse();
    [Test]
    public async Task Delete_One_Match_ReturnsTrue() => await Assert.That(IListExtensions.SortedDelete(ItemsOne, 1, Comparer<int>.Default)).IsTrue();
    [Test]
    public async Task Delete_One_Match_DeletesFromList()
    {
        IListExtensions.SortedDelete(ItemsOne, 1, Comparer<int>.Default);
        await Assert.That(ItemsOne).IsEmpty();
    }
    [Test]
    public async Task Delete_AllFound_Forward()
    {
        foreach (var item in TenEven.ToList())
        {
            await Assert.That(IListExtensions.SortedDelete(TenEven, item, Comparer<int>.Default)).IsTrue();
        }

        await Assert.That(TenEven).IsEmpty();
    }
    [Test]
    public async Task Delete_AllFound_Reverse()
    {
        foreach (var item in TenEven.AsEnumerable().Reverse().ToList())
        {
            await Assert.That(IListExtensions.SortedDelete(TenEven, item, Comparer<int>.Default)).IsTrue();
        }

        await Assert.That(TenEven).IsEmpty();
    }
    [Test]
    public async Task Delete_AllFound_Random()
    {
        foreach (var item in TenEven.OrderBy(Random.Next).ToList())
        {
            await Assert.That(IListExtensions.SortedDelete(TenEven, item, Comparer<int>.Default)).IsTrue();
        }

        await Assert.That(TenEven).IsEmpty();
    }
    [Test]
    public async Task Delete_NoneFound()
    {
        var items = TenEven.ToList();
        foreach (var item in items)
        {
            await Assert.That(IListExtensions.SortedDelete(TenEven, item - 1, Comparer<int>.Default)).IsFalse();
            await Assert.That(IListExtensions.SortedDelete(TenEven, item + 1, Comparer<int>.Default)).IsFalse();
        }
        await Assert.That(TenEven).IsEquivalentTo(items);
    }

    [Test]
    public async Task Insert_Empty_ReturnsTrue() => await Assert.That(IListExtensions.SortedInsert(Items, 0, Comparer<int>.Default)).IsTrue();
    [Test]
    public async Task Insert_Empty_AddsToList()
    {
        IListExtensions.SortedInsert(Items, 0, Comparer<int>.Default);
        await Assert.That(ItemsZero).IsEquivalentTo(Items);
    }
    [Test]
    public async Task Insert_One_Match_ReturnsFalse() => await Assert.That(IListExtensions.SortedInsert(ItemsOne, 1, Comparer<int>.Default)).IsFalse();

    [Test]
    public async Task Insert_AllFound_DoesNothing()
    {
        var items = TenEven.ToList();
        foreach (var item in items)
        {
            await Assert.That(IListExtensions.SortedInsert(TenEven, item, Comparer<int>.Default)).IsFalse();
        }
        await Assert.That(TenEven).IsEquivalentTo(items);
    }
    [Test]
    public async Task Insert_TenEven_Forward_IsSorted()
    {
        foreach (var item in TenEven)
        {
            await Assert.That(IListExtensions.SortedInsert(Items, item, Comparer<int>.Default)).IsTrue();
        }
        await Assert.That(TenEven).IsEquivalentTo(Items);
    }
    [Test]
    public async Task Insert_TenEven_Reverse_IsSorted()
    {
        foreach (var item in TenEven.AsEnumerable().Reverse())
        {
            await Assert.That(IListExtensions.SortedInsert(Items, item, Comparer<int>.Default)).IsTrue();
        }
        await Assert.That(TenEven).IsEquivalentTo(Items);
    }
    [Test]
    public async Task Insert_TenEven_Random_IsSorted()
    {
        foreach (var item in TenEven.OrderBy(Random.Next))
        {
            await Assert.That(IListExtensions.SortedInsert(Items, item, Comparer<int>.Default)).IsTrue();
        }
        await Assert.That(TenEven).IsEquivalentTo(Items);
    }
}