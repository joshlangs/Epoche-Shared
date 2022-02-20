using System.Runtime.CompilerServices;

namespace Epoche.Shared;
public static class ObjectIdHelper
{
    static readonly ConditionalWeakTable<object, ObjectData> Table = new();
    static ObjectData CreateObjectData(object _) => new();

    class ObjectData
    {
        static long LastId = long.MinValue;
        public readonly long Id = Interlocked.Increment(ref LastId);
    }

    /// <summary>
    /// Gets a stable and unique ID to an object
    /// </summary>
    public static long GetId<T>(T obj) where T : class => obj is null ? long.MinValue : Table.GetValue(obj, CreateObjectData).Id;
}
