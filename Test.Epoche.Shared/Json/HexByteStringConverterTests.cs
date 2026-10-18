using System.Text.Json;

namespace Epoche.Shared.Json;

public class HexByteStringConverterTests
{
    static readonly JsonSerializerOptions DefaultJsonSerializerOptions = new()
    {
        Converters = { HexByteStringConverter.Default }
    };
    static readonly JsonSerializerOptions AllowEmptyJsonSerializerOptions = new()
    {
        Converters = { new HexByteStringConverter(32, 32, true) }
    };

    class TestObj
    {
        public string A { get; set; } = default!;
    }

    [Test]
    public async Task Read_String_ReturnsValue()
    {
        var obj = JsonSerializer.Deserialize<TestObj>(@"{""A"":""1234567890abcdef1234567890abcdef1234567890abcdef1234567890abcdef""}", DefaultJsonSerializerOptions);
        await Assert.That(obj!.A).IsEqualTo("1234567890abcdef1234567890abcdef1234567890abcdef1234567890abcdef");
    }

    [Test]
    public async Task Read_UppercaseString_ReturnsValue()
    {
        var obj = JsonSerializer.Deserialize<TestObj>(@"{""A"":""1234567890Abcdef1234567890abcdef1234567890abcdef1234567890abcdeF""}", DefaultJsonSerializerOptions);
        await Assert.That(obj!.A).IsEqualTo("1234567890abcdef1234567890abcdef1234567890abcdef1234567890abcdef");
    }

    [Test]
    public async Task Read_0xString_ReturnsValue()
    {
        var obj = JsonSerializer.Deserialize<TestObj>(@"{""A"":""0x1234567890abcdef1234567890abcdef1234567890abcdef1234567890abcdef""}", DefaultJsonSerializerOptions);
        await Assert.That(obj!.A).IsEqualTo("1234567890abcdef1234567890abcdef1234567890abcdef1234567890abcdef");
    }

    [Test]
    public async Task Read_EmptyString_Throws() => Assert.Throws<Exception>(() => JsonSerializer.Deserialize<TestObj>(@"{""A"":""""}", DefaultJsonSerializerOptions));

    [Test]
    public async Task Read_EmptyString_ReturnsEmpty()
    {
        var obj = JsonSerializer.Deserialize<TestObj>(@"{""A"":""""}", AllowEmptyJsonSerializerOptions);
        await Assert.That(obj!.A).IsEqualTo("");
    }

    [Test]
    public async Task Read_InvalidString_Throws() => Assert.Throws<Exception>(() => JsonSerializer.Deserialize<TestObj>(@"{""A"":""az""}", DefaultJsonSerializerOptions));

    [Test]
    public async Task Read_NonString_Throws() => Assert.Throws<Exception>(() => JsonSerializer.Deserialize<TestObj>(@"{""A"":1}", DefaultJsonSerializerOptions));

    [Test]
    public async Task Read_TooShort_Throws() => Assert.Throws<Exception>(() => JsonSerializer.Deserialize<TestObj>(@"{""A"":""1234567890abcdef1234567890abcdef1234567890abcdef1234567890abcd""}", DefaultJsonSerializerOptions));

    [Test]
    public async Task Read_TooLong_Throws() => Assert.Throws<Exception>(() => JsonSerializer.Deserialize<TestObj>(@"{""A"":""1234567890abcdef1234567890abcdef1234567890abcdef1234567890abcdef00""}", DefaultJsonSerializerOptions));
}
