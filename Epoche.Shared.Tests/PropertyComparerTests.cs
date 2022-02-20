using System.Linq;
using Xunit;

namespace Epoche.Shared;
public class PropertyComparerTests
{
    class TestClass
    {
        public int X { get; init; }
    }

    readonly TestClass[] Items = new[]
    {
        new TestClass { X = 3 },
        new TestClass { X = 1 },
        new TestClass { X = 5 },
        new TestClass { X = 2 },
        new TestClass { X = 4 }
    };

    readonly PropertyComparer<TestClass, int> Ascending = new(x => x.X, true);
    readonly PropertyComparer<TestClass, int> Descending = new(x => x.X, false);

    [Fact]
    public void SortAscending()
    {
        var sorted = Items.ToList();
        sorted.Sort(Ascending);
        Assert.Equal(Items.Select(x => x.X).OrderBy(x => x), sorted.Select(x => x.X));
    }

    [Fact]
    public void SortDescending()
    {
        var sorted = Items.ToList();
        sorted.Sort(Descending);
        Assert.Equal(Items.Select(x => x.X).OrderByDescending(x => x), sorted.Select(x => x.X));
    }

    [Fact]
    public void DuplicatePropertiesNotEqual()
    {
        Assert.NotEqual(0, Ascending.Compare(new TestClass { X = 1 }, new TestClass { X = 1 }));
    }

    [Fact]
    public void DuplicatePropertiesEqual()
    {
        var comp = new PropertyComparer<TestClass, int>(x => x.X, objectsEqualIfPropertiesEqual: true);
        Assert.Equal(0, comp.Compare(new TestClass { X = 1 }, new TestClass { X = 1 }));
    }
}
