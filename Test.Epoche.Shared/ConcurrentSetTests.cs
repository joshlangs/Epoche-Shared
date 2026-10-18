namespace Epoche.Shared;
public class ConcurrentSetTests
{
    readonly ConcurrentSet<int> Empty = new();

    ConcurrentSet<int> Create() => new([1, 3, 5]);

    [Test]
    public async Task Empty_IsEmpty() => await Assert.That(Empty.IsEmpty).IsTrue();

    [Test]
    public async Task Empty_Count0() => await Assert.That(Empty.Count).IsEqualTo(0);

    [Test]
    public async Task NonEmpty_IsNotEmpty() => await Assert.That(Create().IsEmpty).IsFalse();

    [Test]
    public async Task NonEmpty_Count3() => await Assert.That(Create().Count).IsEqualTo(3);

    [Test]
    public async Task Add_New_Adds()
    {
        var items = Create();
        await Assert.That(items.Add(2)).IsTrue();
        await Assert.That(items.Count).IsEqualTo(4);
        await Assert.That(items.Contains(2)).IsTrue();
    }

    [Test]
    public async Task Add_Duplicate_NoEffect()
    {
        var items = Create();
        await Assert.That(items.Add(1)).IsFalse();
        await Assert.That(items.Count).IsEqualTo(3);
        await Assert.That(items.Contains(3)).IsTrue();
    }

    [Test]
    public async Task Clear_Clears()
    {
        var items = Create();
        items.Clear();
        await Assert.That(items.IsEmpty).IsTrue();
        await Assert.That(items.Count).IsEqualTo(0);
    }

    [Test]
    public async Task Remove_Existing_Removes()
    {
        var items = Create();
        await Assert.That(items.Remove(1)).IsTrue();
        await Assert.That(items.Count).IsEqualTo(2);
        await Assert.That(items.Contains(1)).IsFalse();
    }

    [Test]
    public async Task Remove_NonExisting_NoEffect()
    {
        var items = Create();
        await Assert.That(items.Remove(2)).IsFalse();
        await Assert.That(items.Count).IsEqualTo(3);
        await Assert.That(items.Contains(2)).IsFalse();
    }

    [Test]
    public async Task Empty_Equals_Empty() => await Assert.That(Array.Empty<int>()).IsEquivalentTo(Empty);

    [Test]
    public async Task NonEmpty_Equals_NonEmpty() => await Assert.That(Create()).IsEquivalentTo(Create());
}
