namespace Epoche.Shared;
public static class ArrayExtensions
{
    /// <summary>
    /// Reverses the array IN-PLACE, and returns it
    /// </summary>
    public static byte[] ReverseArrayInPlace(this byte[] array)
    {
        ArgumentNullException.ThrowIfNull(array);
        Array.Reverse(array);
        return array;
    }

    /// <summary>
    /// Makes a new array by concatenating two arrays
    /// </summary>
    public static byte[] ConcatArray(this byte[] array, byte[] other)
    {
        ArgumentNullException.ThrowIfNull(array);
        ArgumentNullException.ThrowIfNull(other);

        var buf = new byte[array.Length + other.Length];
        Array.Copy(array, 0, buf, 0, array.Length);
        Array.Copy(other, 0, buf, array.Length, other.Length);
        return buf;
    }
}