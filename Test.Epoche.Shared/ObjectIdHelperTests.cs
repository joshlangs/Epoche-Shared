namespace Epoche.Shared;

public class ObjectIdHelperTests
{
    readonly object Obj1 = new();
    readonly object Obj2 = new();

    readonly string Str1 = "";
    readonly string Str2 = "a";

    [Test]
    public async Task Null_Same() => await Assert.That(ObjectIdHelper.GetId((object)null!)).IsEqualTo(ObjectIdHelper.GetId((object)null!));

    [Test]
    public async Task DifferentObjects_DifferentIds() => await Assert.That(ObjectIdHelper.GetId(Obj1)).IsNotEqualTo(ObjectIdHelper.GetId(Obj2));

    [Test]
    public async Task SameObjects_SameIds()
    {
        await Assert.That(ObjectIdHelper.GetId(Obj1)).IsEqualTo(ObjectIdHelper.GetId(Obj1));
        await Assert.That(ObjectIdHelper.GetId(Obj2)).IsEqualTo(ObjectIdHelper.GetId(Obj2));
    }

    [Test]
    public async Task SameStrings_SameIds()
    {
        await Assert.That(ObjectIdHelper.GetId(Str1)).IsEqualTo(ObjectIdHelper.GetId(Str1));
        await Assert.That(ObjectIdHelper.GetId(Str2)).IsEqualTo(ObjectIdHelper.GetId(Str2));
    }

    [Test]
    public async Task DifferentStrings_DifferentIds() => await Assert.That(ObjectIdHelper.GetId(Str1)).IsNotEqualTo(ObjectIdHelper.GetId(Str2));

    [Test]
    public async Task CastStringToObject_SameIds() => await Assert.That(ObjectIdHelper.GetId((object)Str1)).IsEqualTo(ObjectIdHelper.GetId(Str1));
}
