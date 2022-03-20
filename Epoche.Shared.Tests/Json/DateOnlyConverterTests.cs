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

    [Fact]
    public void Read_EmptyString_Throws() => Assert.ThrowsAny<Exception>(() => JsonSerializer.Deserialize<TestObj>(@"{""A"":""""}", JsonSerializerOptions));

    [Fact]
    public void Read_InvalidString_Throws() => Assert.ThrowsAny<Exception>(() => JsonSerializer.Deserialize<TestObj>(@"{""A"":""a""}", JsonSerializerOptions));

    [Fact]
    public void Read_NonString_Throws() => Assert.ThrowsAny<Exception>(() => JsonSerializer.Deserialize<TestObj>(@"{""A"":1}", JsonSerializerOptions));

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
