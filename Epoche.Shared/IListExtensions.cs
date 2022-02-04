namespace Epoche.Shared;
public static class IListExtensions
{
    public static bool RemoveFirst<T>(this IList<T> items, Func<T, bool> predicate)
    {
        var count = items.Count;
        for (var x = 0; x < count; ++x)
        {
            if (predicate(items[x]))
            {
                items.RemoveAt(x);
                return true;
            }
        }
        return false;
    }
    public static int RemoveAll<T>(this IList<T> items, Func<T, bool> predicate)
    {
        var indexes = new List<int>();
        var count = items.Count;
        for (var x = 0; x < count; ++x)
        {
            if (predicate(items[x]))
            {
                indexes.Add(x);
            }
        }
        for (var x = indexes.Count - 1; x >= 0; --x)
        {
            items.RemoveAt(indexes[x]);
        }
        return indexes.Count;
    }
}
