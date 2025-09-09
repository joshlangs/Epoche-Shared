using System.Text;

namespace Epoche.Shared;
public static class Base32
{
    // https://code.google.com/p/cc-sharp/source/browse/trunk/trunk/src/Transcoder.cs?r=2
    // With modifications :)

    const int IN_BYTE_SIZE = 8;
    const int OUT_BYTE_SIZE = 5;
    static readonly char[] Alphabet = "ABCDEFGHIJKLMNOPQRSTUVWXYZ234567".ToCharArray();

    public static string Encode(byte[] data, bool withPadding = false)
    {
        ArgumentNullException.ThrowIfNull(data);

        int i = 0, index = 0, digit;
        int current_byte, next_byte;
        var result = new StringBuilder((data.Length + 7) * IN_BYTE_SIZE / OUT_BYTE_SIZE);

        while (i < data.Length)
        {
            current_byte = (data[i] >= 0) ? data[i] : (data[i] + 256); // Unsign

            /* Is the current digit going to span a byte boundary? */
            if (index > (IN_BYTE_SIZE - OUT_BYTE_SIZE))
            {
                if ((i + 1) < data.Length)
                {
                    next_byte = (data[i + 1] >= 0) ? data[i + 1] : (data[i + 1] + 256);
                }
                else
                {
                    next_byte = 0;
                }

                digit = current_byte & (0xFF >> index);
                index = (index + OUT_BYTE_SIZE) % IN_BYTE_SIZE;
                digit <<= index;
                digit |= next_byte >> (IN_BYTE_SIZE - index);
                i++;
            }
            else
            {
                digit = (current_byte >> (IN_BYTE_SIZE - (index + OUT_BYTE_SIZE))) & 0x1F;
                index = (index + OUT_BYTE_SIZE) % IN_BYTE_SIZE;
                if (index == 0)
                {
                    i++;
                }
            }
            result.Append(Alphabet[digit]);
        }

        if (withPadding && (result.Length % IN_BYTE_SIZE) != 0)
        {
            result.Append('=', 8 - (result.Length % IN_BYTE_SIZE));
        }

        return result.ToString();
    }
}