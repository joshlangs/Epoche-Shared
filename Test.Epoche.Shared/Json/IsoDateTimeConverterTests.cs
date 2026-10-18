using System.Text.Json;

namespace Epoche.Shared.Json;

public sealed class IsoDateTimeConverterTests
{
    static readonly JsonSerializerOptions JsonSerializerOptions = new()
    {
        Converters = { new IsoDateTimeConverter() }
    };

    class TestObj
    {
        public DateTime A { get; set; }
    }

    [Test]
    public async Task Read_String_ReturnsValue()
    {
        var now = DateTime.UtcNow;
        foreach (var format in IsoDateCheater.GetFormats())
        {
            var formatted = now.ToString(format);
            var obj = JsonSerializer.Deserialize<TestObj>(@"{""A"":""" + formatted + @"""}", JsonSerializerOptions);
            await Assert.That(obj!.A).IsEqualTo(IsoDateCheater.Parse(formatted));
        }
    }

    [Test]
    public async Task Read_EmptyString_Throws() => Assert.Throws<Exception>(() => JsonSerializer.Deserialize<TestObj>(@"{""A"":""""}", JsonSerializerOptions));

    [Test]
    public async Task Read_InvalidString_Throws() => Assert.Throws<Exception>(() => JsonSerializer.Deserialize<TestObj>(@"{""A"":""a""}", JsonSerializerOptions));

    [Test]
    public async Task Read_NonString_Throws() => Assert.Throws<Exception>(() => JsonSerializer.Deserialize<TestObj>(@"{""A"":1}", JsonSerializerOptions));

    [Test]
    public async Task Write_Roundtrips()
    {
        var now = DateTime.UtcNow;
        var test = new TestObj { A = now };
        var s = JsonSerializer.Serialize(test, JsonSerializerOptions);
        await Assert.That(s).Contains(now.ToString("O"));
        var obj = JsonSerializer.Deserialize<TestObj>(s, JsonSerializerOptions);
        await Assert.That(test.A).IsEqualTo(obj!.A);
    }

    [Test]
    public async Task Write_UsesOFormatting()
    {
        var date = new DateTime(2000, 1, 1, 0, 0, 0, DateTimeKind.Utc);
        var test = new TestObj { A = date };
        var s = JsonSerializer.Serialize(test, JsonSerializerOptions);
        await Assert.That(s).Contains(date.ToString("O"));
        var obj = JsonSerializer.Deserialize<TestObj>(s, JsonSerializerOptions);
        await Assert.That(test.A).IsEqualTo(obj!.A);
    }

    [Test]
    public async Task Write_HandlesUnspecifiedKind()
    {
        var date = new DateTime(2000, 1, 1, 0, 0, 0, DateTimeKind.Unspecified);
        var test = new TestObj { A = date };
        var s = JsonSerializer.Serialize(test, JsonSerializerOptions);
        await Assert.That(s).Contains(date.ToString("O"));
        var obj = JsonSerializer.Deserialize<TestObj>(s, JsonSerializerOptions);
        await Assert.That(test.A).IsEqualTo(obj!.A);
    }

    [Test]
    public async Task Write_HandlesLocalKind()
    {
        var date = new DateTime(2000, 1, 1, 0, 0, 0, DateTimeKind.Utc);
        var test = new TestObj { A = date.ToLocalTime() };
        var s = JsonSerializer.Serialize(test, JsonSerializerOptions);
        await Assert.That(s).Contains(date.ToString("O"));
        var obj = JsonSerializer.Deserialize<TestObj>(s, JsonSerializerOptions);
        await Assert.That(date).IsEqualTo(obj!.A);
    }
}
