using System;
using System.Text.Json;
using Xunit;

namespace Epoche.Shared.Json;

public class ByteReadOnlyMemoryConverterTests
{
    static readonly JsonSerializerOptions JsonSerializerOptions = new()
    {
        Converters = { new ByteReadOnlyMemoryConverter() }
    };

    class TestObj
    {
        public ReadOnlyMemory<byte> A { get; set; } = default!;
    }

    [Fact]
    public void Read_String_ReturnsValue()
    {
        var obj = JsonSerializer.Deserialize<TestObj>(@"{""A"":""0af8""}", JsonSerializerOptions);
        Assert.Equal(new byte[] { 0xa, 0xf8 }, obj?.A.ToArray());
    }

    [Fact]
    public void Read_EmptyString_ReturnsEmptyArray()
    {
        var obj = JsonSerializer.Deserialize<TestObj>(@"{""A"":""""}", JsonSerializerOptions);
        Assert.Equal(Array.Empty<byte>(), obj?.A.ToArray());
    }

    [Fact]
    public void Read_InvalidString_Throws() => Assert.ThrowsAny<Exception>(() => JsonSerializer.Deserialize<TestObj>(@"{""A"":""az""}", JsonSerializerOptions));

    [Fact]
    public void Read_NonString_Throws() => Assert.ThrowsAny<Exception>(() => JsonSerializer.Deserialize<TestObj>(@"{""A"":1}", JsonSerializerOptions));

    [Fact]
    public void Write_Roundtrips()
    {
        var test = new TestObj { A = new([0xe3, 0x70]) };
        var s = JsonSerializer.Serialize(test, JsonSerializerOptions);
        Assert.Contains(@"""e370""", s);
        var obj = JsonSerializer.Deserialize<TestObj>(s, JsonSerializerOptions);
        Assert.Equal(obj?.A.ToArray(), test.A.ToArray());
    }
}
