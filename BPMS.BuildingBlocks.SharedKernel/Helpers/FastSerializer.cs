using System.Collections.Concurrent;
using System.Reflection;
using System.Text;

namespace BPMS.BuildingBlocks.SharedKernel.Helpers;

public static class FastSerializer
{
    private static readonly ConcurrentDictionary<Type, IDictionary<PropertyInfo, Type>> typeMetadatas = new();

    private const string PAIR_DELIMITER = "-";
    private const string ITEM_DELIMITER = "|";
    private const string PAIR_REPLACEMENT = "$R45$";
    private const string ITEM_REPLACEMENT = "$R124$";

    private static string ToSafe(string str)
    {
        str = str.Replace(PAIR_DELIMITER, PAIR_REPLACEMENT);
        str = str.Replace(ITEM_DELIMITER, ITEM_REPLACEMENT);
        return str;
    }

    private static string FromSafe(string str)
    {
        str = str.Replace(PAIR_REPLACEMENT, PAIR_DELIMITER);
        str = str.Replace(ITEM_REPLACEMENT, ITEM_DELIMITER);
        return str;
    }

    public static string Serialize(this Dictionary<string, string> dictionary)
    {
        var rtn = new StringBuilder();
        var enumerator = dictionary.GetEnumerator();
        while (enumerator.MoveNext())
            rtn.Append($"{ToSafe(enumerator.Current.Key)}{PAIR_DELIMITER}{ToSafe(enumerator.Current.Value ?? string.Empty)}{ITEM_DELIMITER}");
        return rtn.ToString();
    }

    public static Dictionary<string, string> Deserialize(this string str)
    {
        var rtn = new Dictionary<string, string>();

        var enumerator = (str ?? string.Empty).
            Split(ITEM_DELIMITER, StringSplitOptions.RemoveEmptyEntries).
            Where(item => !string.IsNullOrEmpty(item) && item.Contains(PAIR_DELIMITER)).
            Select(item => item.Split(PAIR_DELIMITER)).
            GetEnumerator();

        while (enumerator.MoveNext())
            rtn.Add(FromSafe(enumerator.Current[0]), FromSafe(enumerator.Current[1]));

        return rtn;
    }

    public static Type MaterializeTypeMetadata(Type type)
    {
        if (!type.IsClass || type.IsAbstract || type.IsInterface)
            throw new InvalidOperationException("the only object types are are allowed to use this serialization is class types");

        if (!type.GetConstructors().Any(ctor => !ctor.GetParameters().Any()))
            throw new InvalidOperationException("the only object types are are allowed to use this serialization is class which ows public paremeterless constructor");

        if (!typeMetadatas.ContainsKey(type))
            typeMetadatas.TryAdd(type, type.GetProperties().ToDictionary(proprty => proprty, property =>
            {
                if (property.PropertyType.IsClass && property.PropertyType != typeof(string))
                    throw new InvalidOperationException("composite object types are not allowed to use this serialization");

                return property.PropertyType;
            }));

        return type;
    }

    public static Type MaterializeTypeMetadata<T>()
    {
        var type = typeof(T);

        return MaterializeTypeMetadata(type);
    }

    public static string Serialize(this object @object)
    {
        Type type = MaterializeTypeMetadata(@object.GetType());

        var dictionary = new Dictionary<string, string>();

        var enumerator = typeMetadatas[type].GetEnumerator();
        while (enumerator.MoveNext())
        {
            object? propertyValue = enumerator.Current.Key.GetValue(@object, null);
            if (propertyValue == null)
                continue;

            string value;
            if (enumerator.Current.Key.PropertyType == typeof(DateTime) || enumerator.Current.Key.PropertyType == typeof(DateTime?))
                value = ((DateTime?)propertyValue).Value.Ticks.ToString();
            else
                value = propertyValue.ToString()!;


            dictionary.Add(enumerator.Current.Key.Name, value);
        }

        return dictionary.Serialize();
    }

    public static T Deserialize<T>(this string str)
    {
        Type type = MaterializeTypeMetadata<T>();

        var dictionary = str.Deserialize();

        var rtn = Activator.CreateInstance(typeof(T))!;

        var enumerator = typeMetadatas[type].GetEnumerator();
        while (enumerator.MoveNext())
        {
            if (!dictionary.ContainsKey(enumerator.Current.Key.Name) || string.IsNullOrEmpty(dictionary[enumerator.Current.Key.Name]))
                continue;

            if (!enumerator.Current.Key.CanWrite)
                continue;

            object value;
            if (enumerator.Current.Key.PropertyType == typeof(string))
                value = dictionary[enumerator.Current.Key.Name];
            else if (enumerator.Current.Key.PropertyType == typeof(DateTime) || enumerator.Current.Key.PropertyType == typeof(DateTime?))
                value = new DateTime(long.Parse(dictionary[enumerator.Current.Key.Name]));
            else
                value = Convert.ChangeType(dictionary[enumerator.Current.Key.Name], enumerator.Current.Key.PropertyType);

            enumerator.Current.Key.SetValue(rtn, value);
        }

        return (T)rtn!;
    }
}