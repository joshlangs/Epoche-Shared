using System.Text.Json;

namespace Epoche.Shared.Json;

public class ByteArrayConverterTests
{
    static readonly JsonSerializerOptions JsonSerializerOptions = new()
    {
        Converters = { new ByteArrayConverter() }
    };

    class TestObj
    {
        public byte[] A { get; set; } = default!;
    }

    [Test]
    public async Task Read_String_ReturnsValue()
    {
        var obj = JsonSerializer.Deserialize<TestObj>(@"{""A"":""0af8""}", JsonSerializerOptions);
        await Assert.That(obj!.A).IsEquivalentTo(new byte[] { 0xa, 0xf8 }); 
    }

    [Test]
    public async Task Read_EmptyString_ReturnsEmptyArray()
    {
        var obj = JsonSerializer.Deserialize<TestObj>(@"{""A"":""""}", JsonSerializerOptions);
        await Assert.That(obj!.A!).IsEquivalentTo(Array.Empty<byte>());
    }

    [Test]
    public async Task Read_InvalidString_Throws() => Assert.Throws<Exception>(() => JsonSerializer.Deserialize<TestObj>(@"{""A"":""az""}", JsonSerializerOptions));

    [Test]
    public async Task Read_NonString_Throws() => Assert.Throws<Exception>(() => JsonSerializer.Deserialize<TestObj>(@"{""A"":1}", JsonSerializerOptions));

    [Test]
    public async Task Write_Roundtrips()
    {
        var test = new TestObj { A = [0xe3, 0x70] };
        var s = JsonSerializer.Serialize(test, JsonSerializerOptions);
        await Assert.That(s).Contains(@"""e370""");
        var obj = JsonSerializer.Deserialize<TestObj>(s, JsonSerializerOptions);
        await Assert.That(test.A).IsEquivalentTo(obj!.A);
    }
}
