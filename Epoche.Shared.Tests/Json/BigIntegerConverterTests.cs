using System;
using System.Numerics;
using System.Text.Json;
using Xunit;

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

    [Fact]
    public void Read_String_ReturnsValue()
    {
        var obj = JsonSerializer.Deserialize<TestObj>(@"{""A"":""-123""}", JsonSerializerOptions);
        Assert.Equal(new BigInteger(-123), obj?.A);
    }


    [Theory]
    [InlineData("\"\"")]
    [InlineData("1")]
    [InlineData("null")]
    [InlineData("[]")]
    [InlineData("{}")]
    [InlineData("\"+1\"")]
    [InlineData("\"1.2\"")]
    [InlineData("\" 1\"")]
    [InlineData("\"1 \"")]
    [InlineData("\"$1\"")]
    [InlineData("\"1,234\"")]
    [InlineData("\"1e3\"")]
    [InlineData("\"1\0\"")]
    public void Read_Invalid_Throws(string value) => Assert.ThrowsAny<Exception>(() => JsonSerializer.Deserialize<TestObj>($@"{{""A"":{value}}}", JsonSerializerOptions));

    void RoundTrips(BigInteger value)
    {
        var test = new TestObj { A = value };
        var s = JsonSerializer.Serialize(test, JsonSerializerOptions);
        Assert.Contains($@"""{value}""", s);
        var obj = JsonSerializer.Deserialize<TestObj>(s, JsonSerializerOptions);
        Assert.Equal(obj?.A, test.A);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    [InlineData(1)]
    [InlineData(1234)]
    [InlineData(-1234)]
    public void Write_Roundtrips(BigInteger value)
    {
        RoundTrips(value);
        RoundTrips(value * 1000000000000000L);
        RoundTrips(value * 1234987612349875L);
    }
}
