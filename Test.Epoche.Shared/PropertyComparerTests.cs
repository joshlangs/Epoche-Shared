namespace Epoche.Shared;

public class PropertyComparerTests
{
    class TestClass
    {
        public int X { get; init; }
    }

    readonly TestClass[] Items =
    [
        new TestClass { X = 3 },
        new TestClass { X = 1 },
        new TestClass { X = 5 },
        new TestClass { X = 2 },
        new TestClass { X = 4 }
    ];

    readonly PropertyComparer<TestClass, int> Ascending = new(x => x.X, true);
    readonly PropertyComparer<TestClass, int> Descending = new(x => x.X, false);

    [Test]
    public async Task SortAscending()
    {
        var sorted = Items.ToList();
        sorted.Sort(Ascending);
        await Assert.That(sorted.Select(x => x.X)).IsEquivalentTo(Items.Select(x => x.X).OrderBy(x => x));
    }

    [Test]
    public async Task SortDescending()
    {
        var sorted = Items.ToList();
        sorted.Sort(Descending);
        await Assert.That(sorted.Select(x => x.X)).IsEquivalentTo(Items.Select(x => x.X).OrderByDescending(x => x));
    }

    [Test]
    public async Task DuplicatePropertiesNotEqual()
    {
        await Assert.That(Ascending.Compare(new TestClass { X = 1 }, new TestClass { X = 1 })).IsNotEqualTo(0);
    }

    [Test]
    public async Task DuplicatePropertiesEqual()
    {
        var comp = new PropertyComparer<TestClass, int>(x => x.X, objectsEqualIfPropertiesEqual: true);
        await Assert.That(comp.Compare(new TestClass { X = 1 }, new TestClass { X = 1 })).IsEqualTo(0);
    }
}
