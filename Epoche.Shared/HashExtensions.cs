using System.Security.Cryptography;

namespace Epoche.Shared;
public static class HashExtensions
{
    public static byte[] ComputeSHA256(this byte[] data) => SHA256.HashData(data);
    public static byte[] ComputeSHA256(this ArraySegment<byte> data) => SHA256.HashData((ReadOnlySpan<byte>)data.AsSpan());
    public static byte[] ComputeSHA256(this Span<byte> data) => SHA256.HashData((ReadOnlySpan<byte>)data);
    public static byte[] ComputeSHA256(this byte[] data, int offset, int count) => SHA256.HashData(data.AsSpan(offset, count));
    public static byte[] ComputeSHA256(this ReadOnlySpan<byte> data) => SHA256.HashData(data);
    public static void ComputeSHA256(this Span<byte> data, Span<byte> hashDestination) => ComputeSHA256((ReadOnlySpan<byte>)data, hashDestination);
    public static void ComputeSHA256(this ReadOnlySpan<byte> data, Span<byte> hashDestination)
    {
        if (hashDestination.Length != 32)
        {
            throw new InvalidOperationException("hashDestination must be exactly 32 bytes");
        }
        if (!SHA256.TryHashData(data, hashDestination, out var written) || written != 32)
        {
            throw new NotImplementedException("This was unexpected");
        }
    }
}