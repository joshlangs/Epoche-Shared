namespace Epoche.Shared;

/// <summary>
/// Suitable for 64 byte or shorter byte arrays.
/// Longer arrays are vulnerable to hashcode collisions.
/// </summary>
public sealed class ByteArrayEqualityComparer : IEqualityComparer<byte[]?>
{
    /// <summary>
    /// Suitable for 64 byte or shorter byte arrays.
    /// Longer arrays are vulnerable to hashcode collisions.
    /// </summary>
    public static readonly ByteArrayEqualityComparer Instance = new ByteArrayEqualityComparer();
    ByteArrayEqualityComparer()
    {
    }
    public bool Equals(byte[]? x, byte[]? y)
    {
        if (ReferenceEquals(x, y))
        {
            return true;
        }

        if (x is null || y is null)
        {
            return x is null && y is null;
        }

        return x.AsSpan().SequenceEqual(y);
    }

    public int GetHashCode(byte[]? obj)
    {
        if (obj is null || obj.Length == 0)
        {
            return 0;
        }
        if (obj.Length == 32)
        {
            return HashCode.Combine(
                BitConverter.ToInt64(obj, 0),
                BitConverter.ToInt64(obj, 8),
                BitConverter.ToInt64(obj, 16),
                BitConverter.ToInt64(obj, 24));
        }
        if (obj.Length == 64)
        {
            return HashCode.Combine(
                BitConverter.ToInt64(obj, 0),
                BitConverter.ToInt64(obj, 8),
                BitConverter.ToInt64(obj, 16),
                BitConverter.ToInt64(obj, 24),
                BitConverter.ToInt64(obj, 32),
                BitConverter.ToInt64(obj, 40),
                BitConverter.ToInt64(obj, 48),
                BitConverter.ToInt64(obj, 56));
        }
        if (obj.Length == 16)
        {
            return HashCode.Combine(
                BitConverter.ToInt64(obj, 0),
                BitConverter.ToInt64(obj, 8));
        }
        var hashCode = new HashCode();
        hashCode.Add(obj.Length);
        var x = 0;
        for (; x <= obj.Length - 8; x += 8)
        {
            hashCode.Add(BitConverter.ToInt64(obj, x));
        }
        for (; x <= obj.Length - 4; x += 4)
        {
            hashCode.Add(BitConverter.ToInt32(obj, x));
        }
        for (; x < obj.Length; ++x)
        {
            hashCode.Add(obj[x]);
        }
        return hashCode.ToHashCode();
    }
}