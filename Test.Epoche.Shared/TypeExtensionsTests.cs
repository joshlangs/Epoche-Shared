using System.Collections;

namespace Epoche.Shared;
public class TypeExtensionsTests
{
    interface IMeow
    {
    }
    interface IRawr<T>
    {
    }
    class TestObj : IMeow, IRawr<int>, IRawr<string>
    {
    }

    class GenericBase<T, U> { }
    class Subclass1<T> : GenericBase<T, string> { }
    class Subclass2 : Subclass1<int> { }

    [Test]
    public async Task GetGenericInterfaces_TestObj_ReturnsBothInterfaces()
    {
        var interfaces = typeof(TestObj).GetGenericInterfaces(typeof(IRawr<>)).ToList();
        await Assert.That(interfaces).Contains(typeof(IRawr<int>));
        await Assert.That(interfaces).Contains(typeof(IRawr<string>));
        await Assert.That(interfaces.Count).IsEqualTo(2);
    }

    [Test]
    public async Task GetGenericInterfaces_NullType_Throws() => Assert.Throws<ArgumentNullException>(() => ((Type)null!).GetGenericInterfaces(typeof(IRawr<>)));

    [Test]
    public async Task GetGenericInterfaces_NullInterface_Throws() => Assert.Throws<ArgumentNullException>(() => typeof(TestObj).GetGenericInterfaces(null!));

    [Test]
    public async Task GetGenericInterfaces_NonGenericInterface_Throws() => Assert.Throws<InvalidOperationException>(() => typeof(TestObj).GetGenericInterfaces(typeof(IEnumerable)));

    [Test]
    public async Task GetGenericInterfaces_NonInterface_Throws() => Assert.Throws<InvalidOperationException>(() => typeof(TestObj).GetGenericInterfaces(typeof(object)));

    [Test]
    public async Task IsSubclassOfOpenGeneric_NotGeneric_Works()
    {
        await Assert.That(typeof(string).IsSubclassOfOpenGeneric(typeof(string))).IsTrue();
        await Assert.That(typeof(string).IsSubclassOfOpenGeneric(typeof(object))).IsTrue();
        await Assert.That(typeof(string).IsSubclassOfOpenGeneric(typeof(TestObj))).IsFalse();
    }

    [Test]
    public async Task IsSubclassOfOpenGeneric_TestCases()
    {
        await Assert.That(typeof(Subclass2).IsSubclassOfOpenGeneric(typeof(GenericBase<,>))).IsTrue();
        await Assert.That(typeof(Subclass1<long>).IsSubclassOfOpenGeneric(typeof(GenericBase<,>))).IsTrue();
        await Assert.That(typeof(GenericBase<,>).IsSubclassOfOpenGeneric(typeof(GenericBase<,>))).IsTrue();
        await Assert.That(typeof(Subclass1<>).IsSubclassOfOpenGeneric(typeof(GenericBase<,>))).IsTrue();
        await Assert.That(typeof(Subclass2).IsSubclassOfOpenGeneric(typeof(GenericBase<,>))).IsTrue();
        await Assert.That(typeof(Subclass2).IsSubclassOfOpenGeneric(typeof(Subclass1<>))).IsTrue();
        await Assert.That(typeof(Subclass1<>).IsSubclassOfOpenGeneric(typeof(Subclass1<>))).IsTrue();
        await Assert.That(typeof(Subclass1<long>).IsSubclassOfOpenGeneric(typeof(Subclass1<>))).IsTrue();
        await Assert.That(typeof(Subclass1<>).IsSubclassOfOpenGeneric(typeof(Subclass1<long>))).IsFalse();
    }
}