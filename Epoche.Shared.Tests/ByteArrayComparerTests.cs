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
        Assert.Equal(-1, ByteArrayComparer.Instance.Compare(Array.Empty<byte>(), null));
        Assert.Equal(1, ByteArrayComparer.Instance.Compare(null, Array.Empty<byte>()));
    }

    [Fact]
    public void EmptyEqualsEmpty() => Assert.Equal(0, ByteArrayComparer.Instance.Compare(Array.Empty<byte>(), Array.Empty<byte>()));

    [Fact]
    public void SameEqualsSame() => Assert.Equal(0, ByteArrayComparer.Instance.Compare(new byte[] { 1, 2, 3 }, new byte[] { 1, 2, 3 }));

    [Fact]
    public void LongerGreaterThanShorter()
    {
        Assert.Equal(-1, ByteArrayComparer.Instance.Compare(new byte[] { 1, 2, 3 }, new byte[] { 1, 2, 3, 4 }));
        Assert.Equal(1, ByteArrayComparer.Instance.Compare(new byte[] { 1, 2, 3, 4 }, new byte[] { 1, 2, 3 }));
    }

    [Fact]
    public void SortedByteOrder()
    {
        Assert.Equal(-1, ByteArrayComparer.Instance.Compare(new byte[] { 0, 2, 3, 4 }, new byte[] { 1, 2, 3 }));
        Assert.Equal(1, ByteArrayComparer.Instance.Compare(new byte[] { 1, 2, 3 }, new byte[] { 0, 2, 3, 4 }));

        Assert.Equal(1, ByteArrayComparer.Instance.Compare(new byte[] { 1, 2, 3, 4 }, new byte[] { 1, 2, 3 }));
        Assert.Equal(-1, ByteArrayComparer.Instance.Compare(new byte[] { 1, 2, 3 }, new byte[] { 1, 2, 3, 4 }));
    }
}
