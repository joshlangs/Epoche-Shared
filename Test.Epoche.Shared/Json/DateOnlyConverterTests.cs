using System.Text.Json;

namespace Epoche.Shared.Json;

public class DateOnlyConverterTests
{
    static readonly JsonSerializerOptions JsonSerializerOptions = new()
    {
        Converters = { new DateOnlyConverter() }
    };

    class TestObj
    {
        public DateOnly A { get; set; }
    }

    [Test]
    public async Task Read_String_ReturnsValue()
    {
        var obj = JsonSerializer.Deserialize<TestObj>(@"{""A"":""1234-12-10""}", JsonSerializerOptions);
        await Assert.That(obj!.A).IsEqualTo(new DateOnly(1234, 12, 10));
    }

    [Test]
    [Arguments("\"\"")]
    [Arguments("4")]
    [Arguments("null")]
    [Arguments("[]")]
    [Arguments("{}")]
    [Arguments("123-45-678")]
    [Arguments("2025-13-01")]
    [Arguments("2025-00-01")]
    [Arguments("2025-12-00")]
    [Arguments("2025-12-32")]
    [Arguments("2025-02-30")]
    [Arguments("-123-02-05")]
    [Arguments("2025--2-30")]
    [Arguments("2025-02--3")]
    [Arguments("2025-02-15\0")]
    [Arguments("2025-02-15 ")]
    [Arguments(" 2025-02-15")]
    public async Task Read_Invalid_Throws(string value) => Assert.Throws<Exception>(() => JsonSerializer.Deserialize<TestObj>($@"{{""A"":{value}}}", JsonSerializerOptions));

    [Test]
    public async Task Write_Roundtrips()
    {
        var test = new TestObj { A = new DateOnly(123, 9, 4) };
        var s = JsonSerializer.Serialize(test, JsonSerializerOptions);
        await Assert.That(s).Contains(@"""0123-09-04""");
        var obj = JsonSerializer.Deserialize<TestObj>(s, JsonSerializerOptions);
        await Assert.That(test.A).IsEqualTo(obj!.A);
    }
}
