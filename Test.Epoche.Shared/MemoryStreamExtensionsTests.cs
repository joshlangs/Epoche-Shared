namespace Epoche.Shared;
public class MemoryStreamExtensionsTests
{
    [Test]
    public async Task GetDataBuffer_Null_ThrowsArgumentNullException() => Assert.Throws<ArgumentNullException>(() => MemoryStreamExtensions.GetDataBuffer(null!));

    [Test]
    public async Task GetDataBuffer_Empty_ReturnsEmpty() => await Assert.That(new MemoryStream().GetDataBuffer().ToList()).IsEmpty();

    [Test]
    public async Task GetDataBuffer_NonwritableStream_ReturnsSameBytes()
    {
        var bytes = RandomHelper.GetRandomBytes(RandomHelper.GetRandomPositiveInt32() % 1000 + 1);
        await Assert.That(new MemoryStream(bytes, false).GetDataBuffer().AsSpan().ToArray()).IsEquivalentTo(bytes);
    }

    [Test]
    public async Task GetDataBuffer_WritableStream_ReturnsSameBytes()
    {
        var bytes = RandomHelper.GetRandomBytes(RandomHelper.GetRandomPositiveInt32() % 1000 + 1);
        var ms = new MemoryStream();
        ms.Write(bytes);
        await Assert.That(ms.GetDataBuffer().AsSpan().ToArray()).IsEquivalentTo(bytes);
    }

    [Test]
    public async Task GetDataSpan_Null_ThrowsArgumentNullException() => Assert.Throws<ArgumentNullException>(() => MemoryStreamExtensions.GetDataSpan(null!));

    [Test]
    public async Task GetDataSpan_Empty_ReturnsEmpty() => await Assert.That(new MemoryStream().GetDataSpan().ToArray()).IsEmpty();


    [Test]
    public async Task GetDataSpan_NonwritableStream_ReturnsSameBytes()
    {
        var bytes = RandomHelper.GetRandomBytes(RandomHelper.GetRandomPositiveInt32() % 1000 + 1);
        await Assert.That(new MemoryStream(bytes, false).GetDataSpan().ToArray()).IsEquivalentTo(bytes);
    }

    [Test]
    public async Task GetDataSpan_WritableStream_ReturnsSameBytes()
    {
        var bytes = RandomHelper.GetRandomBytes(RandomHelper.GetRandomPositiveInt32() % 1000 + 1);
        var ms = new MemoryStream();
        ms.Write(bytes);
        await Assert.That(ms.GetDataSpan().ToArray()).IsEquivalentTo(bytes);
    }

}