using System.Security.Cryptography;

namespace Epoche.Shared;
public static class HashExtensions
{
    static readonly ThreadLocal<SHA256> SHA256s = new(SHA256.Create);

    public static byte[] ComputeSHA256(this byte[] data) => ComputeSHA256((ReadOnlySpan<byte>)data.AsSpan());
    public static byte[] ComputeSHA256(this ArraySegment<byte> data) => ComputeSHA256((ReadOnlySpan<byte>)data.AsSpan());
    public static byte[] ComputeSHA256(this Span<byte> data) => ComputeSHA256((ReadOnlySpan<byte>)data);
    public static byte[] ComputeSHA256(this byte[] data, int offset, int count) => SHA256s.Value!.ComputeHash(data, offset, count);
    public static byte[] ComputeSHA256(this ReadOnlySpan<byte> data)
    {
        var hash = new byte[32];
        if (SHA256s.Value!.TryComputeHash(data, hash, out var written) &&
            written == 32)
        {
            return hash;
        }
        throw new NotImplementedException("This was unexpected");
    }
    public static void ComputeSHA256(this Span<byte> data, Span<byte> hashDestination) => ComputeSHA256((ReadOnlySpan<byte>)data, hashDestination);
    public static void ComputeSHA256(this ReadOnlySpan<byte> data, Span<byte> hashDestination)
    {
        if (hashDestination.Length != 32)
        {
            throw new InvalidOperationException("hashDestination must be exactly 32 bytes");
        }
        if (!SHA256s.Value!.TryComputeHash(data, hashDestination, out var written) ||
            written != 32)
        {
            throw new NotImplementedException("This was unexpected");
        }
    }
}