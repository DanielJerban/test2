using System.Reflection;

namespace BPMS.BuildingBlocks.SharedKernel.Helpers;

public static class Types
{
    private static ICollection<Type>? types;

    public static ICollection<Type> List()
    {
        return types ??= Assemblies.List().SelectMany(a => a.GetTypes()).ToHashSet();
    }

    public static IList<Type> List(Assembly[] assemblies, params Type[] interfaceTypes)
    {
        var allTypes = List().
            Where(a => assemblies.Contains(a.Assembly)).
            Where(t => interfaceTypes == null || interfaceTypes.Length == 0 || interfaceTypes.Any(it => t.GetInterface(it.Name) != null)).
            ToList();

        return allTypes;
    }

    public static IList<Type> List(params Type[] interfaceTypes)
    {
        var allTypes = List().
            Where(t => interfaceTypes == null || interfaceTypes.Length == 0 || interfaceTypes.Any(it => t.GetInterface(it.Name) != null)).
            ToList();

        return allTypes;
    }

    public static void Reset()
    {
        types = null;
    }
}