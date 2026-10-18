using System.Text.Json;

namespace Epoche.Shared.Json;

public class BigFractionConverterTests
{
    static readonly JsonSerializerOptions JsonSerializerOptions = new()
    {
        Converters = { new BigFractionConverter() }
    };

    class TestObj
    {
        public BigFraction A { get; set; }
    }

    [Test]
    public async Task Read_String_ReturnsValue()
    {
        var obj = JsonSerializer.Deserialize<TestObj>(@"{""A"":""1.23""}", JsonSerializerOptions);
        await Assert.That(obj!.A).IsEqualTo(BigFraction.Parse("1.23"));
    }

    [Test]
    [Arguments("\"\"")]
    [Arguments("1")]
    [Arguments("null")]
    [Arguments("[]")]
    [Arguments("{}")]
    [Arguments("\"+1\"")]
    [Arguments("\" 1\"")]
    [Arguments("\"1 \"")]
    [Arguments("\"$1\"")]
    [Arguments("\"1,234\"")]
    [Arguments("\"1e3\"")]
    [Arguments("\"1\0\"")]
    public async Task Read_Invalid_Throws(string value) => Assert.Throws<Exception>(() => JsonSerializer.Deserialize<TestObj>($@"{{""A"":{value}}}", JsonSerializerOptions));

    async Task RoundTrips(BigFraction value)
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
    public async Task Write_Roundtrips(long v)
    {
        BigFraction value = v;
        await RoundTrips(value);
        await RoundTrips(value * 10000000000000000000L);
        await RoundTrips(value * 12374981325896326358L);
        await RoundTrips(value * 1.467235648943982436m);
        await RoundTrips(value * 0.0000001m);
    }
}
