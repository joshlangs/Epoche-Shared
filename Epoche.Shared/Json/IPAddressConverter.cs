using System.Net;
using System.Net.Sockets;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Epoche.Shared.Json;
public sealed class IPAddressConverter : JsonConverter<IPAddress>
{
    public static readonly IPAddressConverter Instance = new();

    public override IPAddress? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType != JsonTokenType.String)
        {
            throw new InvalidOperationException("Only string can be converted to DateTime with this converter");
        }

        var str = reader.GetString();
        if (IPAddress.TryParse(str, out var value))
        {
            if (value.AddressFamily != AddressFamily.InterNetwork)
            {
                return value;
            }
            // Make sure it's a full IP address
            // https://docs.microsoft.com/en-us/dotnet/api/system.net.ipaddress.tryparse?view=net-6.0
            var dot = str.IndexOf('.', 1);
            if (dot >= 1)
            {
                dot = str.IndexOf('.', dot + 2);
                if (dot >= 1)
                {
                    if (str.IndexOf('.', dot + 2) >= 1)
                    {
                        return value;
                    }
                }
            }
        }

        throw new FormatException("The value could not be parsed into a DateTime");
    }

    public override void Write(Utf8JsonWriter writer, IPAddress value, JsonSerializerOptions options) => writer.WriteStringValue(value.ToString());
}
