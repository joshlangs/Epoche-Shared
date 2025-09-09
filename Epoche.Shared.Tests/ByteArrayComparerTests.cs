using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Epoche.Shared;

public class ByteArrayComparerTests
{
    [Fact]
    public void NullEqualsNull() => Assert.Equal(0, ByteArrayComparer.Instance.Compare(null, null));

    [Fact]
    public void NotNullGreaterThanNull()
    {
        Assert.Equal(-1, ByteArrayComparer.Instance.Compare([], null));
        Assert.Equal(1, ByteArrayComparer.Instance.Compare(null, []));
    }

    [Fact]
    public void EmptyEqualsEmpty() => Assert.Equal(0, ByteArrayComparer.Instance.Compare([], []));

    [Fact]
    public void SameEqualsSame() => Assert.Equal(0, ByteArrayComparer.Instance.Compare([1, 2, 3], [1, 2, 3]));

    [Fact]
    public void LongerGreaterThanShorter()
    {
        Assert.Equal(-1, ByteArrayComparer.Instance.Compare([1, 2, 3], [1, 2, 3, 4]));
        Assert.Equal(1, ByteArrayComparer.Instance.Compare([1, 2, 3, 4], [1, 2, 3]));
    }

    [Fact]
    public void SortedByteOrder()
    {
        Assert.Equal(-1, ByteArrayComparer.Instance.Compare([0, 2, 3, 4], [1, 2, 3]));
        Assert.Equal(1, ByteArrayComparer.Instance.Compare([1, 2, 3], [0, 2, 3, 4]));

        Assert.Equal(1, ByteArrayComparer.Instance.Compare([1, 2, 3, 4], [1, 2, 3]));
        Assert.Equal(-1, ByteArrayComparer.Instance.Compare([1, 2, 3], [1, 2, 3, 4]));
    }
}
