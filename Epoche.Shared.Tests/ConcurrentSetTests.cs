using System;
using Xunit;

namespace Epoche.Shared;
public class ConcurrentSetTests
{
    readonly ConcurrentSet<int> Empty = new();

    ConcurrentSet<int> Create() => new(new[] { 1, 3, 5 });

    [Fact]
    public void Empty_IsEmpty() => Assert.True(Empty.IsEmpty);

    [Fact]
    public void Empty_Count0() => Assert.Equal(0, Empty.Count);

    [Fact]
    public void NonEmpty_IsNotEmpty() => Assert.False(Create().IsEmpty);

    [Fact]
    public void NonEmpty_Count3() => Assert.Equal(3, Create().Count);

    [Fact]
    public void Add_New_Adds()
    {
        var items = Create();
        Assert.True(items.Add(2));
        Assert.Equal(4, items.Count);
        Assert.True(items.Contains(2));
    }

    [Fact]
    public void Add_Duplicate_NoEffect()
    {
        var items = Create();
        Assert.False(items.Add(1));
        Assert.Equal(3, items.Count);
        Assert.True(items.Contains(3));
    }

    [Fact]
    public void Clear_Clears()
    {
        var items = Create();
        items.Clear();
        Assert.True(items.IsEmpty);
        Assert.Equal(0, items.Count);
    }

    [Fact]
    public void Remove_Existing_Removes()
    {
        var items = Create();
        Assert.True(items.Remove(1));
        Assert.Equal(2, items.Count);
        Assert.False(items.Contains(1));
    }

    [Fact]
    public void Remove_NonExisting_NoEffect()
    {
        var items = Create();
        Assert.False(items.Remove(2));
        Assert.Equal(3, items.Count);
        Assert.False(items.Contains(2));
    }

    [Fact]
    public void Empty_Equals_Empty() => Assert.Equal(Empty, Array.Empty<int>());

    [Fact]
    public void NonEmpty_Equals_NonEmpty() => Assert.Equal(Create(), Create());
}
