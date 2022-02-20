using Xunit;

namespace Epoche.Shared;
public class ObjectIdHelperTests
{
    readonly object Obj1 = new();
    readonly object Obj2 = new();

    readonly string Str1 = "";
    readonly string Str2 = "a";

    [Fact]
    public void Null_Same() => Assert.Equal(ObjectIdHelper.GetObjectId((object)null!), ObjectIdHelper.GetObjectId((object)null!));

    [Fact]
    public void DifferentObjects_DifferentIds() => Assert.NotEqual(ObjectIdHelper.GetObjectId(Obj1), ObjectIdHelper.GetObjectId(Obj2));

    [Fact]
    public void SameObjects_SameIds()
    {
        Assert.Equal(ObjectIdHelper.GetObjectId(Obj1), ObjectIdHelper.GetObjectId(Obj1));
        Assert.Equal(ObjectIdHelper.GetObjectId(Obj2), ObjectIdHelper.GetObjectId(Obj2));
    }

    [Fact]
    public void SameStrings_SameIds()
    {
        Assert.Equal(ObjectIdHelper.GetObjectId(Str1), ObjectIdHelper.GetObjectId(Str1));
        Assert.Equal(ObjectIdHelper.GetObjectId(Str2), ObjectIdHelper.GetObjectId(Str2));
    }

    [Fact]
    public void DifferentStrings_DifferentIds() => Assert.NotEqual(ObjectIdHelper.GetObjectId(Str1), ObjectIdHelper.GetObjectId(Str2));

    [Fact]
    public void CastStringToObject_SameIds() => Assert.Equal(ObjectIdHelper.GetObjectId(Str1), ObjectIdHelper.GetObjectId((object)Str1));
}
