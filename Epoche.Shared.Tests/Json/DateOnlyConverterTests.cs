using System;
using System.Text.Json;
using Xunit;

namespace Epoche.Shared.Json;

public class DateOnlyConverterTests
{
    static readonly JsonSerializerOptions JsonSerializerOptions = new()
    {
        Converters = { new DateOnlyConverter() }
    };

    class TestObj
    {
        public DateOnly A { get; set; }
    }

    [Fact]
    public void Read_String_ReturnsValue()
    {
        var obj = JsonSerializer.Deserialize<TestObj>(@"{""A"":""1234-12-10""}", JsonSerializerOptions);
        Assert.Equal(new DateOnly(1234, 12, 10), obj?.A);
    }

    [Theory]
    [InlineData("\"\"")]
    [InlineData("4")]
    [InlineData("null")]
    [InlineData("[]")]
    [InlineData("{}")]
    [InlineData("123-45-678")]
    [InlineData("2025-13-01")]
    [InlineData("2025-00-01")]
    [InlineData("2025-12-00")]
    [InlineData("2025-12-32")]
    [InlineData("2025-02-30")]
    [InlineData("-123-02-05")]
    [InlineData("2025--2-30")]
    [InlineData("2025-02--3")]
    [InlineData("2025-02-15\0")]
    [InlineData("2025-02-15 ")]
    [InlineData(" 2025-02-15")]
    public void Read_Invalid_Throws(string value) => Assert.ThrowsAny<Exception>(() => JsonSerializer.Deserialize<TestObj>($@"{{""A"":{value}}}", JsonSerializerOptions));

    [Fact]
    public void Write_Roundtrips()
    {
        var test = new TestObj { A = new DateOnly(123, 9, 4) };
        var s = JsonSerializer.Serialize(test, JsonSerializerOptions);
        Assert.Contains(@"""0123-09-04""", s);
        var obj = JsonSerializer.Deserialize<TestObj>(s, JsonSerializerOptions);
        Assert.Equal(obj?.A, test.A);
    }
}
