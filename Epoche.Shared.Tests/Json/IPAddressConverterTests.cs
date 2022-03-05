using System;
using System.Net;
using System.Text.Json;
using Xunit;

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

    [Fact]
    public void Read_String_ReturnsValue()
    {
        var obj = JsonSerializer.Deserialize<TestObj>(@"{""A"":""1.2.3.4""}", JsonSerializerOptions);
        Assert.Equal(IPAddress.Parse("1.2.3.4"), obj?.A);
    }

    [Fact]
    public void Read_StringV6_ReturnsValue()
    {
        var obj = JsonSerializer.Deserialize<TestObj>(@"{""A"":""::1""}", JsonSerializerOptions);
        Assert.Equal(IPAddress.Parse("::1"), obj?.A);
    }

    [Fact]
    public void Read_EmptyString_Throws() => Assert.ThrowsAny<Exception>(() => JsonSerializer.Deserialize<TestObj>(@"{""A"":""""}", JsonSerializerOptions));

    [Fact]
    public void Read_InvalidString_Throws() => Assert.ThrowsAny<Exception>(() => JsonSerializer.Deserialize<TestObj>(@"{""A"":""a""}", JsonSerializerOptions));

    [Fact]
    public void Read_Int64String_Throws() => Assert.ThrowsAny<Exception>(() => JsonSerializer.Deserialize<TestObj>(@"{""A"":""123456""}", JsonSerializerOptions));

    [Fact]
    public void Read_1DotString_Throws() => Assert.ThrowsAny<Exception>(() => JsonSerializer.Deserialize<TestObj>(@"{""A"":""1.2""}", JsonSerializerOptions));

    [Fact]
    public void Read_2DotString_Throws() => Assert.ThrowsAny<Exception>(() => JsonSerializer.Deserialize<TestObj>(@"{""A"":""1.2.3""}", JsonSerializerOptions));

    [Fact]
    public void Read_3DotEndingString_Throws() => Assert.ThrowsAny<Exception>(() => JsonSerializer.Deserialize<TestObj>(@"{""A"":""1.2.3.""}", JsonSerializerOptions));

    [Fact]
    public void Read_3DotStartingString_Throws() => Assert.ThrowsAny<Exception>(() => JsonSerializer.Deserialize<TestObj>(@"{""A"":"".1.2.3""}", JsonSerializerOptions));

    [Fact]
    public void Read_3DotMiddleString_Throws() => Assert.ThrowsAny<Exception>(() => JsonSerializer.Deserialize<TestObj>(@"{""A"":""1.2..3""}", JsonSerializerOptions));

    [Fact]
    public void Read_NonString_Throws() => Assert.ThrowsAny<Exception>(() => JsonSerializer.Deserialize<TestObj>(@"{""A"":1}", JsonSerializerOptions));

    [Fact]
    public void Write_Roundtrips()
    {
        var test = new TestObj { A = IPAddress.Parse("123.21.0.55") };
        var s = JsonSerializer.Serialize(test, JsonSerializerOptions);
        Assert.Contains(@"""123.21.0.55""", s);
        var obj = JsonSerializer.Deserialize<TestObj>(s, JsonSerializerOptions);
        Assert.Equal(obj?.A, test.A);
    }

    [Fact]
    public void Write_RoundtripsV6()
    {
        var test = new TestObj { A = IPAddress.Parse("2001:db8:3333:4444:5555:6666:7777:8888") };
        var s = JsonSerializer.Serialize(test, JsonSerializerOptions);
        Assert.Contains(@"""2001:db8:3333:4444:5555:6666:7777:8888""", s);
        var obj = JsonSerializer.Deserialize<TestObj>(s, JsonSerializerOptions);
        Assert.Equal(obj?.A, test.A);
    }
}
