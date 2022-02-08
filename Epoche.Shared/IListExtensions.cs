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

    /// <summary>
    /// Returns true if the item was added.  False if it already existed.
    /// The list must already be in sorted order or else this will produce incorrect results.
    /// </summary>
    public static bool SortedInsert<T>(this IList<T> items, T item, IComparer<T> comparer)
    {
        if (items is null)
        {
            throw new ArgumentNullException(nameof(items));
        }
        if (item is null)
        {
            throw new ArgumentNullException(nameof(item));
        }
        if (comparer is null)
        {
            throw new ArgumentNullException(nameof(comparer));
        }
        var index = items.BinarySearch(item, comparer);
        if (index >= 0)
        {
            return false;
        }
        items.Insert(~index, item);
        return true;
    }
    /// <summary>
    /// Returns true if an item was delete.  False if it was not found.
    /// The list must already be in sorted order or else this will produce incorrect results.
    /// </summary>
    public static bool SortedDelete<T>(this IList<T> items, T item, IComparer<T> comparer)
    {
        if (items is null)
        {
            throw new ArgumentNullException(nameof(items));
        }
        if (item is null)
        {
            throw new ArgumentNullException(nameof(item));
        }
        if (comparer is null)
        {
            throw new ArgumentNullException(nameof(comparer));
        }
        var index = items.BinarySearch(item, comparer);
        if (index < 0)
        {
            return false;
        }
        items.RemoveAt(index);
        return true;
    }
    /// <summary>
    /// Returns the index of the item, or it's compliment if it was not found.
    /// The list must already be in sorted order or else this will produce incorrect results.
    /// </summary>
    public static int BinarySearch<T>(this IList<T> items, T item, IComparer<T> comparer)
    {
        if (items is null)
        {
            throw new ArgumentNullException(nameof(items));
        }
        if (item is null)
        {
            throw new ArgumentNullException(nameof(item));
        }
        if (comparer is null)
        {
            throw new ArgumentNullException(nameof(comparer));
        }
        if (items.Count == 0)
        {
            return ~0;
        }

        var low = 0;
        var high = items.Count - 1;
        while (true)
        {
            var mid = low + (high - low) / 2;
            var cmp = comparer.Compare(item, items[mid]);
            if (cmp == 0)
            {
                return mid;
            }

            if (cmp < 0)
            {
                high = mid - 1;
                if (low > high)
                {
                    return ~mid;
                }
            }
            else
            {
                low = mid + 1;
                if (low > high)
                {
                    return ~(mid + 1);
                }
            }
        }
    }
}
