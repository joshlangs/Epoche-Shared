namespace Epoche.Shared;
public class ByteArrayEqualityComparerTests
{
    [Test]
    public async Task GetHashCode_Null_Returns0() => await Assert.That(ByteArrayEqualityComparer.Instance.GetHashCode(null)).IsEqualTo(0);

    [Test]
    public async Task GetHashCode_Empty_Returns0() => await Assert.That(ByteArrayEqualityComparer.Instance.GetHashCode([])).IsEqualTo(0);

    [Test]
    public async Task GetHashCode_TwoInstances_DifferentCodes()
    {
        for (var x = 0; x < 128; ++x)
        {
            var buf1 = new byte[x];
            var buf2 = buf1.Concat(new byte[] { 0 }).ToArray();
            await Assert.That(ByteArrayEqualityComparer.Instance.GetHashCode(buf1)).IsNotEqualTo(ByteArrayEqualityComparer.Instance.GetHashCode(buf2));
            await Assert.That(ByteArrayEqualityComparer.Instance.Equals(buf1, buf2)).IsFalse();
        }
    }

    [Test]
    public async Task GetHashCode_TwoInstances_EqualSameCodes()
    {
        for (var x = 0; x < 128; ++x)
        {
            var buf1 = RandomHelper.GetRandomBytes(x);
            var buf2 = buf1.ToArray();
            await Assert.That(ByteArrayEqualityComparer.Instance.GetHashCode(buf2)).IsEqualTo(ByteArrayEqualityComparer.Instance.GetHashCode(buf1));
            await Assert.That(ByteArrayEqualityComparer.Instance.Equals(buf1, buf2)).IsTrue();
        }
    }

    [Test]
    public async Task Equals_EqualArrays_ReturnsTrue()
    {
        var bytes = RandomHelper.GetRandomBytes(32);
        await Assert.That(ByteArrayEqualityComparer.Instance.Equals(bytes, [.. bytes])).IsTrue();
    }
    [Test]
    public async Task Equals_UnequalLengthArrays_ReturnsFalse()
    {
        var bytes = RandomHelper.GetRandomBytes(32);
        await Assert.That(ByteArrayEqualityComparer.Instance.Equals(bytes, [.. bytes.Take(31)])).IsFalse();
    }
    [Test]
    public async Task Equals_UnequalArrays_ReturnsFalse()
    {
        var bytes = RandomHelper.GetRandomBytes(32);
        var bytes2 = bytes.ToArray();
        bytes2[31]++;
        await Assert.That(ByteArrayEqualityComparer.Instance.Equals(bytes, bytes2)).IsFalse();
    }

    [Test]
    public async Task Equals_TwoNulls_ReturnsTrue() => await Assert.That(ByteArrayEqualityComparer.Instance.Equals(null, null)).IsTrue();

    [Test]
    public async Task Equals_LeftNull_ReturnsFalse() => await Assert.That(ByteArrayEqualityComparer.Instance.Equals(null, new byte[1])).IsFalse();

    [Test]
    public async Task Equals_RightNull_ReturnsFalse() => await Assert.That(ByteArrayEqualityComparer.Instance.Equals(new byte[1], null)).IsFalse();
}