using System;
using System.Collections;
using System.Linq;
using Xunit;

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

    [Fact]
    [Trait("Type", "Unit")]
    public void GetGenericInterfaces_TestObj_ReturnsBothInterfaces()
    {
        var interfaces = typeof(TestObj).GetGenericInterfaces(typeof(IRawr<>)).ToList();
        Assert.Contains(typeof(IRawr<int>), interfaces);
        Assert.Contains(typeof(IRawr<string>), interfaces);
        Assert.Equal(2, interfaces.Count);
    }

    [Fact]
    [Trait("Type", "Unit")]
    public void GetGenericInterfaces_NullType_Throws() => Assert.Throws<ArgumentNullException>(() => ((Type)null!).GetGenericInterfaces(typeof(IRawr<>)));

    [Fact]
    [Trait("Type", "Unit")]
    public void GetGenericInterfaces_NullInterface_Throws() => Assert.Throws<ArgumentNullException>(() => typeof(TestObj).GetGenericInterfaces(null!));

    [Fact]
    [Trait("Type", "Unit")]
    public void GetGenericInterfaces_NonGenericInterface_Throws() => Assert.Throws<InvalidOperationException>(() => typeof(TestObj).GetGenericInterfaces(typeof(IEnumerable)));

    [Fact]
    [Trait("Type", "Unit")]
    public void GetGenericInterfaces_NonInterface_Throws() => Assert.Throws<InvalidOperationException>(() => typeof(TestObj).GetGenericInterfaces(typeof(object)));

    [Fact]
    [Trait("Type", "Unit")]
    public void IsSubclassOfOpenGeneric_NotGeneric_Works()
    {
        Assert.True(typeof(string).IsSubclassOfOpenGeneric(typeof(string)));
        Assert.True(typeof(string).IsSubclassOfOpenGeneric(typeof(object)));
        Assert.False(typeof(string).IsSubclassOfOpenGeneric(typeof(TestObj)));
    }

    [Fact]
    [Trait("Type", "Unit")]
    public void IsSubclassOfOpenGeneric_TestCases()
    {
        Assert.True(typeof(Subclass2).IsSubclassOfOpenGeneric(typeof(GenericBase<,>)));
        Assert.True(typeof(Subclass1<long>).IsSubclassOfOpenGeneric(typeof(GenericBase<,>)));
        Assert.True(typeof(GenericBase<,>).IsSubclassOfOpenGeneric(typeof(GenericBase<,>)));
        Assert.True(typeof(Subclass1<>).IsSubclassOfOpenGeneric(typeof(GenericBase<,>)));
        Assert.True(typeof(Subclass2).IsSubclassOfOpenGeneric(typeof(GenericBase<,>)));
        Assert.True(typeof(Subclass2).IsSubclassOfOpenGeneric(typeof(Subclass1<>)));
        Assert.True(typeof(Subclass1<>).IsSubclassOfOpenGeneric(typeof(Subclass1<>)));
        Assert.True(typeof(Subclass1<long>).IsSubclassOfOpenGeneric(typeof(Subclass1<>)));
        Assert.False(typeof(Subclass1<>).IsSubclassOfOpenGeneric(typeof(Subclass1<long>)));
    }
}