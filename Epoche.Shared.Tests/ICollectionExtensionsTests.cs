using System;
using System.Collections.Generic;
using Xunit;

namespace Epoche.Shared;
public class ICollectionExtensionsTests
{
    [Fact]
    public void AddRange_AddsAll()
    {
        ICollection<int> collection = new List<int>() { 1, 2, 3 };
        var add = new[] { 4, 5, 6 };
        collection.AddRange(add);
        Assert.Equal(new[] { 1, 2, 3, 4, 5, 6 }, collection);
    }

    [Fact]
    public void AddRange_ReadOnly_Fails() => Assert.Throws<NotSupportedException>(() => new[] { 1 }.AddRange(new[] { 2 }));
}
