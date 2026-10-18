using System.Text.Json;

namespace Epoche.Shared.Json;

public class Int128ConverterTests
{
    static readonly JsonSerializerOptions JsonSerializerOptions = new()
    {
        Converters = { new Int128Converter() }
    };

    class TestObj
    {
        public Int128 A { get; set; }
    }

    [Test]
    public async Task Read_String_ReturnsValue()
    {
        var obj = JsonSerializer.Deserialize<TestObj>(@"{""A"":""123""}", JsonSerializerOptions);
        await Assert.That(obj!.A).IsEqualTo(123);
    }

    [Test]
    [Arguments("\"\"")]
    [Arguments("1")]
    [Arguments("null")]
    [Arguments("[]")]
    [Arguments("{}")]
    [Arguments("\"+1\"")]
    [Arguments("\"1.2\"")]
    [Arguments("\" 1\"")]
    [Arguments("\"1 \"")]
    [Arguments("\"$1\"")]
    [Arguments("\"1,234\"")]
    [Arguments("\"1e3\"")]
    [Arguments("\"1\0\"")]
    public async Task Read_Invalid_Throws(string value) => Assert.Throws<Exception>(() => JsonSerializer.Deserialize<TestObj>($@"{{""A"":{value}}}", JsonSerializerOptions));

    async Task RoundTrips(Int128 value)
    {
        var test = new TestObj { A = value };
        var s = JsonSerializer.Serialize(test, JsonSerializerOptions);
        await Assert.That(s).Contains($@"""{value}""");
        var obj = JsonSerializer.Deserialize<TestObj>(s, JsonSerializerOptions);
        await Assert.That(test.A).IsEqualTo(obj!.A);
    }

    [Test]
    [Arguments(0)]
    [Arguments(1)]
    [Arguments(1234)]
    public async Task Write_Roundtrips(long value) => await RoundTrips(value);

    [Test]
    public async Task Write_MinMax()
    {
        await RoundTrips(Int128.MinValue);
        await RoundTrips(Int128.MaxValue);
    }
}
