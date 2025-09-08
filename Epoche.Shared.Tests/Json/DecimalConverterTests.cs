using System;
using System.Text.Json;
using Xunit;

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

    [Fact]
    public void Read_String_ReturnsValue()
    {
        var obj = JsonSerializer.Deserialize<TestObj>(@"{""A"":""1.23""}", JsonSerializerOptions);
        Assert.Equal(1.23m, obj?.A);
    }

    [Theory]
    [InlineData("\"\"")]
    [InlineData("1")]
    [InlineData("null")]
    [InlineData("[]")]
    [InlineData("{}")]
    [InlineData("\"+1\"")]
    [InlineData("\"0.1.2\"")]
    [InlineData("\" 1\"")]
    [InlineData("\"1 \"")]
    [InlineData("\"$1\"")]
    [InlineData("\"1,234\"")]
    [InlineData("\"1e3\"")]
    [InlineData("\"1\0\"")]
    public void Read_Invalid_Throws(string value) => Assert.ThrowsAny<Exception>(() => JsonSerializer.Deserialize<TestObj>($@"{{""A"":{value}}}", JsonSerializerOptions));

    void RoundTrips(decimal value)
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
    public void Write_Roundtrips(decimal value)
    {
        RoundTrips(value);
        RoundTrips(value / 10);
        RoundTrips(value / 100);
        RoundTrips(value / 10000);
        RoundTrips(value / 17);
        RoundTrips(value * 17.34567m);
    }

    [Fact]
    public void Write_MinMax()
    {
        RoundTrips(decimal.MinValue);
        RoundTrips(decimal.MaxValue);
    }
}
