using System;
using System.Text.Json;
using Xunit;

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

    [Fact]
    public void Read_String_ReturnsValue()
    {
        var obj = JsonSerializer.Deserialize<TestObj>(@"{""A"":""123""}", JsonSerializerOptions);
        Assert.Equal(123, obj?.A);
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

    void RoundTrips(Int128 value)
    {
        var test = new TestObj { A = value };
        var s = JsonSerializer.Serialize(test, JsonSerializerOptions);
        Assert.Contains($@"""{value}""", s);
        var obj = JsonSerializer.Deserialize<TestObj>(s, JsonSerializerOptions);
        Assert.Equal(obj?.A, test.A);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(1)]
    [InlineData(1234)]
    public void Write_Roundtrips(long value) => RoundTrips(value);

    [Fact]
    public void Write_MinMax()
    {
        RoundTrips(Int128.MinValue);
        RoundTrips(Int128.MaxValue);
    }
}
