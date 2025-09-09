using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace Epoche.Shared;
public class IListExtensionsTests_Sorted
{
    readonly List<int> Items = new();

    readonly List<int> ItemsZero = new() { 0 };
    readonly List<int> ItemsOne = new() { 1 };
    readonly List<int> TenEven = [.. Enumerable.Range(0, 10).Select(x => x * 2)];

    readonly Random Random = new();

    [Fact]
    [Trait("Type", "Unit")]
    public void BinarySearch_ItemsNull_ThrowsArgumentNullException() => Assert.Throws<ArgumentNullException>(() => IListExtensions.BinarySearch(null!, 0, Comparer<int>.Default));
    [Fact]
    [Trait("Type", "Unit")]
    public void BinarySearch_ItemNull_ThrowsArgumentNullException() => Assert.Throws<ArgumentNullException>(() => IListExtensions.BinarySearch(new List<object>(), null!, Comparer<object>.Default));
    [Fact]
    [Trait("Type", "Unit")]
    public void BinarySearch_ComparerNull_ThrowsArgumentNullException() => Assert.Throws<ArgumentNullException>(() => IListExtensions.BinarySearch(Items, 0, null!));

    [Fact]
    [Trait("Type", "Unit")]
    public void SortedInsert_ItemsNull_ThrowsArgumentNullException() => Assert.Throws<ArgumentNullException>(() => IListExtensions.SortedInsert(null!, 0, Comparer<int>.Default));
    [Fact]
    [Trait("Type", "Unit")]
    public void SortedInsert_ItemNull_ThrowsArgumentNullException() => Assert.Throws<ArgumentNullException>(() => IListExtensions.SortedInsert(new List<object>(), null!, Comparer<object>.Default));
    [Fact]
    [Trait("Type", "Unit")]
    public void SortedInsert_ComparerNull_ThrowsArgumentNullException() => Assert.Throws<ArgumentNullException>(() => IListExtensions.SortedInsert(Items, 0, null!));

    [Fact]
    [Trait("Type", "Unit")]
    public void SortedDelete_ItemsNull_ThrowsArgumentNullException() => Assert.Throws<ArgumentNullException>(() => IListExtensions.SortedDelete(null!, 0, Comparer<int>.Default));
    [Fact]
    [Trait("Type", "Unit")]
    public void SortedDelete_ItemNull_ThrowsArgumentNullException() => Assert.Throws<ArgumentNullException>(() => IListExtensions.SortedDelete(new List<object>(), null!, Comparer<object>.Default));
    [Fact]
    [Trait("Type", "Unit")]
    public void SortedDelete_ComparerNull_ThrowsArgumentNullException() => Assert.Throws<ArgumentNullException>(() => IListExtensions.SortedDelete(Items, 0, null!));


    [Fact]
    [Trait("Type", "Unit")]
    public void BinarySearch_Empty_ReturnsCmp0() => Assert.Equal(~0, IListExtensions.BinarySearch(Items, 0, Comparer<int>.Default));

    [Fact]
    [Trait("Type", "Unit")]
    public void BinarySearch_Match_Returns0() => Assert.Equal(0, IListExtensions.BinarySearch(ItemsZero, 0, Comparer<int>.Default));

    [Fact]
    [Trait("Type", "Unit")]
    public void BinarySearch_NoMatch_Greater_ReturnsCmp1() => Assert.Equal(~1, IListExtensions.BinarySearch(ItemsZero, 1, Comparer<int>.Default));

    [Fact]
    [Trait("Type", "Unit")]
    public void BinarySearch_NoMatch_Lesser_ReturnsCmp0() => Assert.Equal(~0, IListExtensions.BinarySearch(ItemsOne, 0, Comparer<int>.Default));

    [Fact]
    [Trait("Type", "Unit")]
    public void BinarySearch_AllFound()
    {
        for (var x = 0; x < TenEven.Count; ++x)
        {
            Assert.Equal(x, IListExtensions.BinarySearch(TenEven, TenEven[x], Comparer<int>.Default));
        }
    }
    [Fact]
    [Trait("Type", "Unit")]
    public void BinarySearch_NoneFound_Lesser()
    {
        for (var x = 0; x < TenEven.Count; ++x)
        {
            Assert.Equal(~x, IListExtensions.BinarySearch(TenEven, TenEven[x] - 1, Comparer<int>.Default));
        }
    }
    [Fact]
    [Trait("Type", "Unit")]
    public void BinarySearch_NoneFound_Greater()
    {
        for (var x = 0; x < TenEven.Count; ++x)
        {
            Assert.Equal(~(x + 1), IListExtensions.BinarySearch(TenEven, TenEven[x] + 1, Comparer<int>.Default));
        }
    }

    [Fact]
    [Trait("Type", "Unit")]
    public void Delete_Empty_ReturnsFalse() => Assert.False(IListExtensions.SortedDelete(Items, 0, Comparer<int>.Default));
    [Fact]
    [Trait("Type", "Unit")]
    public void Delete_One_Match_ReturnsTrue() => Assert.True(IListExtensions.SortedDelete(ItemsOne, 1, Comparer<int>.Default));
    [Fact]
    [Trait("Type", "Unit")]
    public void Delete_One_Match_DeletesFromList()
    {
        IListExtensions.SortedDelete(ItemsOne, 1, Comparer<int>.Default);
        Assert.Empty(ItemsOne);
    }
    [Fact]
    [Trait("Type", "Unit")]
    public void Delete_AllFound_Forward()
    {
        foreach (var item in TenEven.ToList())
        {
            Assert.True(IListExtensions.SortedDelete(TenEven, item, Comparer<int>.Default));
        }

        Assert.Empty(TenEven);
    }
    [Fact]
    [Trait("Type", "Unit")]
    public void Delete_AllFound_Reverse()
    {
        foreach (var item in TenEven.AsEnumerable().Reverse().ToList())
        {
            Assert.True(IListExtensions.SortedDelete(TenEven, item, Comparer<int>.Default));
        }

        Assert.Empty(TenEven);
    }
    [Fact]
    [Trait("Type", "Unit")]
    public void Delete_AllFound_Random()
    {
        foreach (var item in TenEven.OrderBy(Random.Next).ToList())
        {
            Assert.True(IListExtensions.SortedDelete(TenEven, item, Comparer<int>.Default));
        }

        Assert.Empty(TenEven);
    }
    [Fact]
    [Trait("Type", "Unit")]
    public void Delete_NoneFound()
    {
        var items = TenEven.ToList();
        foreach (var item in items)
        {
            Assert.False(IListExtensions.SortedDelete(TenEven, item - 1, Comparer<int>.Default));
            Assert.False(IListExtensions.SortedDelete(TenEven, item + 1, Comparer<int>.Default));
        }
        Assert.Equal(items, TenEven);
    }

    [Fact]
    [Trait("Type", "Unit")]
    public void Insert_Empty_ReturnsTrue() => Assert.True(IListExtensions.SortedInsert(Items, 0, Comparer<int>.Default));
    [Fact]
    [Trait("Type", "Unit")]
    public void Insert_Empty_AddsToList()
    {
        IListExtensions.SortedInsert(Items, 0, Comparer<int>.Default);
        Assert.Equal(Items, ItemsZero);
    }
    [Fact]
    [Trait("Type", "Unit")]
    public void Insert_One_Match_ReturnsFalse() => Assert.False(IListExtensions.SortedInsert(ItemsOne, 1, Comparer<int>.Default));

    [Fact]
    [Trait("Type", "Unit")]
    public void Insert_AllFound_DoesNothing()
    {
        var items = TenEven.ToList();
        foreach (var item in items)
        {
            Assert.False(IListExtensions.SortedInsert(TenEven, item, Comparer<int>.Default));
        }
        Assert.Equal(items, TenEven);
    }
    [Fact]
    [Trait("Type", "Unit")]
    public void Insert_TenEven_Forward_IsSorted()
    {
        foreach (var item in TenEven)
        {
            Assert.True(IListExtensions.SortedInsert(Items, item, Comparer<int>.Default));
        }
        Assert.Equal(Items, TenEven);
    }
    [Fact]
    [Trait("Type", "Unit")]
    public void Insert_TenEven_Reverse_IsSorted()
    {
        foreach (var item in TenEven.AsEnumerable().Reverse())
        {
            Assert.True(IListExtensions.SortedInsert(Items, item, Comparer<int>.Default));
        }
        Assert.Equal(Items, TenEven);
    }
    [Fact]
    [Trait("Type", "Unit")]
    public void Insert_TenEven_Random_IsSorted()
    {
        foreach (var item in TenEven.OrderBy(Random.Next))
        {
            Assert.True(IListExtensions.SortedInsert(Items, item, Comparer<int>.Default));
        }
        Assert.Equal(Items, TenEven);
    }
}