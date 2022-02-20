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

    public static long GetObjectId(object obj) => obj is null ? long.MinValue : Table.GetValue(obj, CreateObjectData).Id;
}
