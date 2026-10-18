using System.Net;
using System.Text.Json;

namespace Epoche.Shared.Json;

public class IPAddressConverterTests
{
    static readonly JsonSerializerOptions JsonSerializerOptions = new()
    {
        Converters = { new IPAddressConverter() }
    };

    class TestObj
    {
        public IPAddress A { get; set; } = default!;
    }

    [Test]
    public async Task Read_String_ReturnsValue()
    {
        var obj = JsonSerializer.Deserialize<TestObj>(@"{""A"":""1.2.3.4""}", JsonSerializerOptions);
        await Assert.That(obj!.A).IsEqualTo(IPAddress.Parse("1.2.3.4"));
    }

    [Test]
    public async Task Read_StringV6_ReturnsValue()
    {
        var obj = JsonSerializer.Deserialize<TestObj>(@"{""A"":""::1""}", JsonSerializerOptions);
        await Assert.That(obj!.A).IsEqualTo(IPAddress.Parse("::1"));
    }

    [Test]
    public async Task Read_EmptyString_Throws() => Assert.Throws<Exception>(() => JsonSerializer.Deserialize<TestObj>(@"{""A"":""""}", JsonSerializerOptions));

    [Test]
    public async Task Read_InvalidString_Throws() => Assert.Throws<Exception>(() => JsonSerializer.Deserialize<TestObj>(@"{""A"":""a""}", JsonSerializerOptions));

    [Test]
    public async Task Read_Int64String_Throws() => Assert.Throws<Exception>(() => JsonSerializer.Deserialize<TestObj>(@"{""A"":""123456""}", JsonSerializerOptions));

    [Test]
    public async Task Read_1DotString_Throws() => Assert.Throws<Exception>(() => JsonSerializer.Deserialize<TestObj>(@"{""A"":""1.2""}", JsonSerializerOptions));

    [Test]
    public async Task Read_2DotString_Throws() => Assert.Throws<Exception>(() => JsonSerializer.Deserialize<TestObj>(@"{""A"":""1.2.3""}", JsonSerializerOptions));

    [Test]
    public async Task Read_3DotEndingString_Throws() => Assert.Throws<Exception>(() => JsonSerializer.Deserialize<TestObj>(@"{""A"":""1.2.3.""}", JsonSerializerOptions));

    [Test]
    public async Task Read_3DotStartingString_Throws() => Assert.Throws<Exception>(() => JsonSerializer.Deserialize<TestObj>(@"{""A"":"".1.2.3""}", JsonSerializerOptions));

    [Test]
    public async Task Read_3DotMiddleString_Throws() => Assert.Throws<Exception>(() => JsonSerializer.Deserialize<TestObj>(@"{""A"":""1.2..3""}", JsonSerializerOptions));

    [Test]
    public async Task Read_NonString_Throws() => Assert.Throws<Exception>(() => JsonSerializer.Deserialize<TestObj>(@"{""A"":1}", JsonSerializerOptions));

    [Test]
    public async Task Write_Roundtrips()
    {
        var test = new TestObj { A = IPAddress.Parse("123.21.0.55") };
        var s = JsonSerializer.Serialize(test, JsonSerializerOptions);
        await Assert.That(s).Contains(@"""123.21.0.55""");
        var obj = JsonSerializer.Deserialize<TestObj>(s, JsonSerializerOptions);
        await Assert.That(test.A).IsEqualTo(obj!.A);
    }

    [Test]
    public async Task Write_RoundtripsV6()
    {
        var test = new TestObj { A = IPAddress.Parse("2001:db8:3333:4444:5555:6666:7777:8888") };
        var s = JsonSerializer.Serialize(test, JsonSerializerOptions);
        await Assert.That(s).Contains(@"""2001:db8:3333:4444:5555:6666:7777:8888""");
        var obj = JsonSerializer.Deserialize<TestObj>(s, JsonSerializerOptions);
        await Assert.That(test.A).IsEqualTo(obj!.A);
    }
}
