using System.Text.Json;

namespace Epoche.Shared.Json;

public class DecimalConverterTests
{
    static readonly JsonSerializerOptions JsonSerializerOptions = new()
    {
        Converters = { new DecimalConverter() }
    };

    class TestObj
    {
        public decimal A { get; set; }
    }

    [Test]
    public async Task Read_String_ReturnsValue()
    {
        var obj = JsonSerializer.Deserialize<TestObj>(@"{""A"":""1.23""}", JsonSerializerOptions);
        await Assert.That(obj!.A).IsEqualTo(1.23m);
    }

    [Test]
    [Arguments("\"\"")]
    [Arguments("1")]
    [Arguments("null")]
    [Arguments("[]")]
    [Arguments("{}")]
    [Arguments("\"+1\"")]
    [Arguments("\"0.1.2\"")]
    [Arguments("\" 1\"")]
    [Arguments("\"1 \"")]
    [Arguments("\"$1\"")]
    [Arguments("\"1,234\"")]
    [Arguments("\"1e3\"")]
    [Arguments("\"1\0\"")]
    public async Task Read_Invalid_Throws(string value) => Assert.Throws<Exception>(() => JsonSerializer.Deserialize<TestObj>($@"{{""A"":{value}}}", JsonSerializerOptions));

    async Task RoundTrips(decimal value)
    {
        var test = new TestObj { A = value };
        var s = JsonSerializer.Serialize(test, JsonSerializerOptions);
        await Assert.That(s).Contains($@"""{value}""");
        var obj = JsonSerializer.Deserialize<TestObj>(s, JsonSerializerOptions);
        await Assert.That(test.A).IsEqualTo(obj!.A);
    }

    [Test]
    [Arguments(0)]
    [Arguments(-1)]
    [Arguments(1)]
    [Arguments(1234)]
    [Arguments(-1234)]
    public async Task Write_Roundtrips(decimal value)
    {
        await RoundTrips(value);
        await RoundTrips(value / 10);
        await RoundTrips(value / 100);
        await RoundTrips(value / 10000);
        await RoundTrips(value / 17);
        await RoundTrips(value * 17.34567m);
    }

    [Test]
    public async Task Write_MinMax()
    {
        await RoundTrips(decimal.MinValue);
        await RoundTrips(decimal.MaxValue);
    }
}
