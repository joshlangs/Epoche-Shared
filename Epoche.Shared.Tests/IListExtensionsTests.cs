using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace Epoche.Shared;
public class IListExtensionsTests
{
    readonly int[] Source = new[] { 1, 2, 3, 4, 5 };
    IList<int> GetData() => Source.ToList();

    [Fact]
    [Trait("Type", "Unit")]
    public void RemoveAll_NoMatch_Returns0() => Assert.Equal(0, GetData().RemoveAll(x => false));

    [Fact]
    [Trait("Type", "Unit")]
    public void RemoveAll_AllMatch_Returns5() => Assert.Equal(5, GetData().RemoveAll(x => true));

    [Fact]
    [Trait("Type", "Unit")]
    public void RemoveAll_3Match_Returns3() => Assert.Equal(3, GetData().RemoveAll(x => x == 1 || x == 3 || x == 5));

    [Fact]
    [Trait("Type", "Unit")]
    public void RemoveAll_NoMatch_ListUnchanged()
    {
        var data = GetData();
        data.RemoveAll(x => false);
        Assert.Equal(GetData(), data);
    }

    [Fact]
    [Trait("Type", "Unit")]
    public void RemoveAll_AllMatch_ListEmpty()
    {
        var data = GetData();
        data.RemoveAll(x => true);
        Assert.Empty(data);
    }

    [Fact]
    [Trait("Type", "Unit")]
    public void RemoveAll_3Match_RemainingCorrect()
    {
        var data = GetData();
        data.RemoveAll(x => x == 1 || x == 3 || x == 5);
        Assert.Equal(new[] { 2, 4 }, data);
    }

    [Fact]
    [Trait("Type", "Unit")]
    public void RemoveFirst_NoMatch_False() => Assert.False(GetData().RemoveFirst(x => false));

    [Fact]
    [Trait("Type", "Unit")]
    public void RemoveFirst_AllMatch_True() => Assert.True(GetData().RemoveFirst(x => true));

    [Fact]
    [Trait("Type", "Unit")]
    public void RemoveFirst_SingleMatch_True() => Assert.True(GetData().RemoveFirst(x => x == 3));

    [Fact]
    [Trait("Type", "Unit")]
    public void RemoveFirst_NoMatch_ListUnchanged()
    {
        var data = GetData();
        data.RemoveFirst(x => false);
        Assert.Equal(GetData(), data);
    }

    [Fact]
    [Trait("Type", "Unit")]
    public void RemoveFirst_AllMatch_RemoveFirst()
    {
        var data = GetData();
        data.RemoveFirst(x => true);
        Assert.Equal(new[] { 2, 3, 4, 5 }, data);
    }

    [Fact]
    [Trait("Type", "Unit")]
    public void RemoveFirst_ThirdMatch_RemoveThird()
    {
        var data = GetData();
        data.RemoveFirst(x => x == 3);
        Assert.Equal(new[] { 1, 2, 4, 5 }, data);
    }
}
