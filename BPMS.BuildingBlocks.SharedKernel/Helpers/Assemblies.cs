using System.Reflection;

namespace BPMS.BuildingBlocks.SharedKernel.Helpers;

public static class Assemblies
{
    private static ICollection<Assembly>? assemblies = null;

    public static ICollection<Assembly> List()
    {
        if (assemblies == null)
            assemblies = AppDomain.CurrentDomain.GetAssemblies().ToHashSet();

        return assemblies;
    }

    public static IList<Assembly> List(params Type[] parentTypes)
    {
        var allTypes = List().
            Where(a => a.GetTypes().Any(t => t.GetInterfaces().Any(i => parentTypes.Contains(i.IsGenericType ? i.GetGenericTypeDefinition() : i)))).
            ToList();

        return allTypes;
    }

    public static void Reset()
    {
        assemblies = null;
    }
}