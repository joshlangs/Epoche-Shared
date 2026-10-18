namespace Epoche.Shared;
public class ICollectionExtensionsTests
{
    [Test]
    public async Task AddRange_AddsAll()
    {
        ICollection<int> collection = new List<int>() { 1, 2, 3 };
        var add = new[] { 4, 5, 6 };
        collection.AddRange(add);
        await Assert.That(collection).IsEquivalentTo([1, 2, 3, 4, 5, 6]);
    }

    [Test]
    public async Task AddRange_ReadOnly_Fails() => Assert.Throws<NotSupportedException>(() => new[] { 1 }.AddRange([2]));
}
