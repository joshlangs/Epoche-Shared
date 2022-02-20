namespace Epoche.Shared;
public sealed class PropertyComparer<T, TProperty> : IComparer<T> where T : class
{
    readonly bool Ascending;
    readonly Func<T, TProperty> GetProperty;
    readonly IComparer<TProperty> Comparer;
    readonly bool ObjectsEqualIfPropertiesEqual;

    public PropertyComparer(Func<T, TProperty> getProperty, bool ascending = true, IComparer<TProperty>? comparer = null, bool objectsEqualIfPropertiesEqual = false)
    {
        GetProperty = getProperty ?? throw new ArgumentNullException(nameof(getProperty));
        Ascending = ascending;
        Comparer = comparer ?? Comparer<TProperty>.Default;
        ObjectsEqualIfPropertiesEqual = objectsEqualIfPropertiesEqual;
    }

    public int Compare(T? x, T? y)
    {
        if (ReferenceEquals(x, y))
        {
            return 0;
        }
        if (x is null || y is null)
        {
            return x is null ? 1 : -1;
        }

        var px = GetProperty(x);
        var py = GetProperty(y);

        if (ReferenceEquals(px, py))
        {
            return ObjectsEqualIfPropertiesEqual ? 0 : CompareEqual(x, y);
        }
        if (px is null || py is null)
        {
            return px is null ? 1 : -1;
        }

        var cmp = Comparer.Compare(px, py);
        if (cmp == 0)
        {
            return ObjectsEqualIfPropertiesEqual ? 0 : CompareEqual(x, y);
        }
        return Ascending ? cmp : -cmp;
    }

    int CompareEqual(T x, T y)
    {
        var cmp =
            x is IComparable<T> c ? c.CompareTo(y) :
            y is IComparable<T> c2 ? -c2.CompareTo(x) :
            ObjectIdHelper.GetId(x).CompareTo(ObjectIdHelper.GetId(y));
        return Ascending ? cmp : -cmp;
    }
}
