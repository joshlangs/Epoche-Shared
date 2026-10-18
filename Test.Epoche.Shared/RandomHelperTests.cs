namespace Epoche.Shared;

public class RandomHelperTests
{
    [Test]
    public async Task GetRandomBytes_Negative_ThrowsArgumentOutOfRangeException() => Assert.Throws<ArgumentOutOfRangeException>(() => RandomHelper.GetRandomBytes(-1));

    [Test]
    public async Task GetRandomBytes_Zero_ReturnsEmpty() => await Assert.That(RandomHelper.GetRandomBytes(0)).IsEmpty();

    [Test]
    public async Task GetRandomBytes_6_Returns6Bytes() => await Assert.That(RandomHelper.GetRandomBytes(6).Length).IsEqualTo(6);

    [Test]
    public async Task GetRandomBytes_BytesAreDifferent() => await Assert.That(RandomHelper.GetRandomBytes(16).Distinct().Count() > 1).IsTrue();

    [Test]
    public async Task GetRandomBytes_Null_ThrowsArgumentNullException() => Assert.Throws<ArgumentNullException>(() => RandomHelper.GetRandomBytes(null!));

    [Test]
    public async Task GetRandomInt64_Repeated_IsDifferent() => await Assert.That(Enumerable.Range(0, 4).Select(x => RandomHelper.GetRandomInt64()).Distinct().Count() > 1).IsTrue();

    [Test]
    public async Task GetRandomInt32_Repeated_IsDifferent() => await Assert.That(Enumerable.Range(0, 4).Select(x => RandomHelper.GetRandomInt32()).Distinct().Count() > 1).IsTrue();

    [Test]
    public async Task GetRandomPositiveInt64_Repeated_IsDifferent() => await Assert.That(Enumerable.Range(0, 4).Select(x => RandomHelper.GetRandomPositiveInt64()).Distinct().Count() > 1).IsTrue();

    [Test]
    public async Task GetRandomPositiveInt32_Repeated_IsDifferent() => await Assert.That(Enumerable.Range(0, 4).Select(x => RandomHelper.GetRandomPositiveInt32()).Distinct().Count() > 1).IsTrue();

    [Test]
    public async Task GetRandomPositiveInt64_IsPositive() => await Assert.That(RandomHelper.GetRandomPositiveInt64() > 0).IsTrue();

    [Test]
    public async Task GetRandomPositiveInt32_IsPositive() => await Assert.That(RandomHelper.GetRandomPositiveInt32() > 0).IsTrue();

    [Test]
    public async Task GetRandomBytes_EmptyArray_DoesNotThrow() => RandomHelper.GetRandomBytes(Array.Empty<byte>());

    [Test]
    public async Task GetRandomBytes_EmptySpan_DoesNotThrow() => RandomHelper.GetRandomBytes([]);

    [Test]
    public async Task GetRandomBytes_ByteArray_BytesAreDifferent()
    {
        var data = new byte[16];
        RandomHelper.GetRandomBytes(data);
        await Assert.That(data.Distinct().Count() > 1).IsTrue();
    }

    [Test]
    public async Task GetRandomBytes_ByteSpan_BytesAreDifferent()
    {
        var data = new byte[16];
        RandomHelper.GetRandomBytes(data.AsSpan());
        await Assert.That(data.Distinct().Count() > 1).IsTrue();
    }

    [Test]
    public async Task GetRandomLowerHex_Repeated_IsDifferent() => await Assert.That(RandomHelper.GetRandomLowerHex(32)).IsNotEqualTo(RandomHelper.GetRandomLowerHex(32));

    [Test]
    public async Task GetRandomLowerHex_Length_Correct() => await Assert.That(Enumerable.Range(0, 16)).IsEquivalentTo(Enumerable.Range(0, 16).Select(x => RandomHelper.GetRandomLowerHex(x).Length));

    [Test]
    public async Task GetRandomLowerHex_ReturnsHex()
    {
        for (var x = 0; x < 16; ++x)
        {
            var str = RandomHelper.GetRandomLowerHex(x);
            await Assert.That(str.All(x => (x >= '0' && x <= '9') || (x >= 'a' && x <= 'f'))).IsTrue();
        }
    }
}
