using Xunit;

namespace Epoche.Shared;
public class ObjectIdHelperTests
{
    readonly object Obj1 = new();
    readonly object Obj2 = new();

    readonly string Str1 = "";
    readonly string Str2 = "a";

    [Fact]
    public void Null_Same() => Assert.Equal(ObjectIdHelper.GetId((object)null!), ObjectIdHelper.GetId((object)null!));

    [Fact]
    public void DifferentObjects_DifferentIds() => Assert.NotEqual(ObjectIdHelper.GetId(Obj1), ObjectIdHelper.GetId(Obj2));

    [Fact]
    public void SameObjects_SameIds()
    {
        Assert.Equal(ObjectIdHelper.GetId(Obj1), ObjectIdHelper.GetId(Obj1));
        Assert.Equal(ObjectIdHelper.GetId(Obj2), ObjectIdHelper.GetId(Obj2));
    }

    [Fact]
    public void SameStrings_SameIds()
    {
        Assert.Equal(ObjectIdHelper.GetId(Str1), ObjectIdHelper.GetId(Str1));
        Assert.Equal(ObjectIdHelper.GetId(Str2), ObjectIdHelper.GetId(Str2));
    }

    [Fact]
    public void DifferentStrings_DifferentIds() => Assert.NotEqual(ObjectIdHelper.GetId(Str1), ObjectIdHelper.GetId(Str2));

    [Fact]
    public void CastStringToObject_SameIds() => Assert.Equal(ObjectIdHelper.GetId(Str1), ObjectIdHelper.GetId((object)Str1));
}
