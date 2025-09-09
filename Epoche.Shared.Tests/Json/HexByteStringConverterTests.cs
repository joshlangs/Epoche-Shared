using System;
using System.Text.Json;
using Xunit;

namespace Epoche.Shared.Json;

public class HexByteStringConverterTests
{
    static readonly JsonSerializerOptions DefaultJsonSerializerOptions = new()
    {
        Converters = { HexByteStringConverter.Default }
    };
    static readonly JsonSerializerOptions AllowEmptyJsonSerializerOptions = new()
    {
        Converters = { new HexByteStringConverter(32, 32, true) }
    };

    class TestObj
    {
        public string A { get; set; } = default!;
    }

    [Fact]
    public void Read_String_ReturnsValue()
    {
        var obj = JsonSerializer.Deserialize<TestObj>(@"{""A"":""1234567890abcdef1234567890abcdef1234567890abcdef1234567890abcdef""}", DefaultJsonSerializerOptions);
        Assert.Equal("1234567890abcdef1234567890abcdef1234567890abcdef1234567890abcdef", obj?.A);
    }

    [Fact]
    public void Read_UppercaseString_ReturnsValue()
    {
        var obj = JsonSerializer.Deserialize<TestObj>(@"{""A"":""1234567890Abcdef1234567890abcdef1234567890abcdef1234567890abcdeF""}", DefaultJsonSerializerOptions);
        Assert.Equal("1234567890abcdef1234567890abcdef1234567890abcdef1234567890abcdef", obj?.A);
    }

    [Fact]
    public void Read_0xString_ReturnsValue()
    {
        var obj = JsonSerializer.Deserialize<TestObj>(@"{""A"":""0x1234567890abcdef1234567890abcdef1234567890abcdef1234567890abcdef""}", DefaultJsonSerializerOptions);
        Assert.Equal("1234567890abcdef1234567890abcdef1234567890abcdef1234567890abcdef", obj?.A);
    }

    [Fact]
    public void Read_EmptyString_Throws() => Assert.ThrowsAny<Exception>(() => JsonSerializer.Deserialize<TestObj>(@"{""A"":""""}", DefaultJsonSerializerOptions));

    [Fact]
    public void Read_EmptyString_ReturnsEmpty()
    {
        var obj = JsonSerializer.Deserialize<TestObj>(@"{""A"":""""}", AllowEmptyJsonSerializerOptions);
        Assert.Equal("", obj?.A);
    }

    [Fact]
    public void Read_InvalidString_Throws() => Assert.ThrowsAny<Exception>(() => JsonSerializer.Deserialize<TestObj>(@"{""A"":""az""}", DefaultJsonSerializerOptions));

    [Fact]
    public void Read_NonString_Throws() => Assert.ThrowsAny<Exception>(() => JsonSerializer.Deserialize<TestObj>(@"{""A"":1}", DefaultJsonSerializerOptions));

    [Fact]
    public void Read_TooShort_Throws() => Assert.ThrowsAny<Exception>(() => JsonSerializer.Deserialize<TestObj>(@"{""A"":""1234567890abcdef1234567890abcdef1234567890abcdef1234567890abcd""}", DefaultJsonSerializerOptions));

    [Fact]
    public void Read_TooLong_Throws() => Assert.ThrowsAny<Exception>(() => JsonSerializer.Deserialize<TestObj>(@"{""A"":""1234567890abcdef1234567890abcdef1234567890abcdef1234567890abcdef00""}", DefaultJsonSerializerOptions));
}
