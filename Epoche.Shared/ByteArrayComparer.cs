namespace Epoche.Shared;
public sealed class ByteArrayComparer : IComparer<byte[]?>
{
    public static readonly ByteArrayComparer Instance = new ByteArrayComparer();
    ByteArrayComparer() { }

    public int Compare(byte[]? x, byte[]? y)
    {
        if (x is null || y is null)
        {
            if (x is null && y is null)
            {
                return 0;
            }
            else if (x is null)
            {
                return 1;
            }
            else
            {
                return -1;
            }
        }

        return x.AsSpan().SequenceCompareTo(y);
    }
}