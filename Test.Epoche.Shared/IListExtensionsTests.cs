namespace Epoche.Shared;

public class IListExtensionsTests
{
    readonly int[] Source = [1, 2, 3, 4, 5];
    IList<int> GetData() => [.. Source];

    [Test]
    public async Task RemoveAll_NoMatch_Returns0() => await Assert.That(GetData().RemoveAll(x => false)).IsEqualTo(0);

    [Test]
    public async Task RemoveAll_AllMatch_Returns5() => await Assert.That(GetData().RemoveAll(x => true)).IsEqualTo(5);

    [Test]
    public async Task RemoveAll_3Match_Returns3() => await Assert.That(GetData().RemoveAll(x => x == 1 || x == 3 || x == 5)).IsEqualTo(3);

    [Test]
    public async Task RemoveAll_NoMatch_ListUnchanged()
    {
        var data = GetData();
        data.RemoveAll(x => false);
        await Assert.That(data).IsEquivalentTo(GetData());
    }

    [Test]
    public async Task RemoveAll_AllMatch_ListEmpty()
    {
        var data = GetData();
        data.RemoveAll(x => true);
        await Assert.That(data).IsEmpty();
    }

    [Test]
    public async Task RemoveAll_3Match_RemainingCorrect()
    {
        var data = GetData();
        data.RemoveAll(x => x == 1 || x == 3 || x == 5);
        await Assert.That(data).IsEquivalentTo([2, 4]);
    }

    [Test]
    public async Task RemoveFirst_NoMatch_False() => await Assert.That(GetData().RemoveFirst(x => false)).IsFalse();

    [Test]
    public async Task RemoveFirst_AllMatch_True() => await Assert.That(GetData().RemoveFirst(x => true)).IsTrue();

    [Test]
    public async Task RemoveFirst_SingleMatch_True() => await Assert.That(GetData().RemoveFirst(x => x == 3)).IsTrue();

    [Test]
    public async Task RemoveFirst_NoMatch_ListUnchanged()
    {
        var data = GetData();
        data.RemoveFirst(x => false);
        await Assert.That(data).IsEquivalentTo(GetData());
    }

    [Test]
    public async Task RemoveFirst_AllMatch_RemoveFirst()
    {
        var data = GetData();
        data.RemoveFirst(x => true);
        await Assert.That(data).IsEquivalentTo([2, 3, 4, 5]);
    }

    [Test]
    public async Task RemoveFirst_ThirdMatch_RemoveThird()
    {
        var data = GetData();
        data.RemoveFirst(x => x == 3);
        await Assert.That(data).IsEquivalentTo([1, 2, 4, 5]);
    }
}
