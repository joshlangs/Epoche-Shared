using System.Collections;
using System.Collections.Concurrent;

namespace Epoche.Shared;
public sealed class ConcurrentSet<T>: IEnumerable<T> where T : notnull
{
    readonly ConcurrentDictionary<T, IntPtr> Dictionary;

    public bool IsEmpty => Dictionary.IsEmpty;
    public int Count => Dictionary.Count;

    public ConcurrentSet()
    {
        Dictionary = new();
    }
    public ConcurrentSet(IEqualityComparer<T> equalityComparer)
    {
        Dictionary = new(equalityComparer);
    }
    public ConcurrentSet(IEnumerable<T> items)
    {
        Dictionary = new(items.Select(x => new KeyValuePair<T, IntPtr>(x, IntPtr.Zero)));
    }
    public ConcurrentSet(IEnumerable<T> items, IEqualityComparer<T> equalityComparer)
    {
        Dictionary = new(items.Select(x => new KeyValuePair<T, IntPtr>(x, IntPtr.Zero)), equalityComparer);
    }

    public bool Add(T item) => Dictionary.TryAdd(item, IntPtr.Zero);
    public bool Remove(T item) => Dictionary.TryRemove(item, out _);
    public void Clear() => Dictionary.Clear();
    public bool Contains(T item) => Dictionary.ContainsKey(item);
    public IEnumerator<T> GetEnumerator() => Dictionary.Keys.GetEnumerator();
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}
