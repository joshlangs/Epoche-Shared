using System;
using System.Numerics;
using System.Text.Json;
using Xunit;

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

    [Fact]
    public void Read_String_ReturnsValue()
    {
        var obj = JsonSerializer.Deserialize<TestObj>(@"{""A"":""1.23""}", JsonSerializerOptions);
        Assert.Equal(BigFraction.Parse("1.23"), obj?.A);
    }

    [Theory]
    [InlineData("\"\"")]
    [InlineData("1")]
    [InlineData("null")]
    [InlineData("[]")]
    [InlineData("{}")]
    [InlineData("\"+1\"")]
    [InlineData("\" 1\"")]
    [InlineData("\"1 \"")]
    [InlineData("\"$1\"")]
    [InlineData("\"1,234\"")]
    [InlineData("\"1e3\"")]
    [InlineData("\"1\0\"")]
    public void Read_Invalid_Throws(string value) => Assert.ThrowsAny<Exception>(() => JsonSerializer.Deserialize<TestObj>($@"{{""A"":{value}}}", JsonSerializerOptions));

    void RoundTrips(BigFraction value)
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
    public void Write_Roundtrips(long v)
    {
        BigFraction value = v;
        RoundTrips(value);
        RoundTrips(value * 10000000000000000000L);
        RoundTrips(value * 12374981325896326358L);
        RoundTrips(value * 1.467235648943982436m);
        RoundTrips(value * 0.0000001m);
    }
}
