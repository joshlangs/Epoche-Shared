namespace Epoche.Shared;
public static class TypeExtensions
{
    public static IEnumerable<Type> GetGenericInterfaces(this Type type, Type genericInterfaceType)
    {
        ArgumentNullException.ThrowIfNull(type);
        ArgumentNullException.ThrowIfNull(genericInterfaceType);
        if (!genericInterfaceType.IsInterface || !genericInterfaceType.IsGenericType)
        {
            throw new InvalidOperationException($"{nameof(genericInterfaceType)} must be a generic interface type");
        }
        return type
            .GetInterfaces()
            .Where(x => x.IsGenericType)
            .Where(x => x.GetGenericTypeDefinition() == genericInterfaceType);
    }

    public static bool IsSubclassOfOpenGeneric(this Type type, Type genericType) =>
        type is null || genericType is null ? false :
        type == genericType ? true :
        type.IsGenericType && type.GetGenericTypeDefinition() == genericType ? true :
        IsSubclassOfOpenGeneric(type.BaseType!, genericType);
}