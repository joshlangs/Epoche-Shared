using System.Numerics;
using System.Text.Json;

namespace Epoche.Shared.Json;

public class BigIntegerConverterTests
{
    static readonly JsonSerializerOptions JsonSerializerOptions = new()
    {
        Converters = { new BigIntegerConverter() }
    };

    class TestObj
    {
        public BigInteger A { get; set; }
    }

    [Test]
    public async Task Read_String_ReturnsValue()
    {
        var obj = JsonSerializer.Deserialize<TestObj>(@"{""A"":""-123""}", JsonSerializerOptions);
        await Assert.That(obj!.A).IsEqualTo(new BigInteger(-123));
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

    async Task RoundTrips(BigInteger value)
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
    public async Task Write_Roundtrips(BigInteger value)
    {
        await RoundTrips(value);
        await RoundTrips(value * 1000000000000000L);
        await RoundTrips(value * 1234987612349875L);
    }
}
