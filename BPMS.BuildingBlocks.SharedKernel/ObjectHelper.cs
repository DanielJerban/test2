using BPMS.BuildingBlocks.SharedKernel.Helpers.Classes;
using Newtonsoft.Json;

namespace BPMS.BuildingBlocks.SharedKernel;

public static class ObjectHelper
{
    public static string ToJson<T>(this T input, Formatting formatting = Formatting.Indented)
    {
        return JsonConvert.SerializeObject(input, formatting, new JsonSerializerSettings() { ReferenceLoopHandling = ReferenceLoopHandling.Ignore });
    }

#pragma warning disable CS8602 // Dereference of a possibly null reference.
#pragma warning disable CS8603 // Possible null reference return.
    public static string ToSafeString<T>(this T? obj, T @default)
    {
        if (obj != null)
            return obj.ToString();

        if (!@default.Equals(default))
            return @default.ToString();

        return string.Empty;
    }

    public static string ToSafeString(this object? obj, int decimals = 2)
    {
        if (obj != null)
        {
            if (obj is decimal dec)
                return Math.Round(dec, decimals).ToString("0.##");

            return obj.ToString();
        }

        return string.Empty;
    }
#pragma warning restore CS8603 // Possible null reference return.
#pragma warning restore CS8602 // Dereference of a possibly null reference.

    public static string Name(this Type type)
    {
        var rtn = type.Name;
        if (type.GetGenericArguments().Any())
        {
            rtn = type.Name.Substring(0, rtn.IndexOf('`'));
            foreach (var t in type.GetGenericArguments())
            {
                var str = t.Name;

                if (t.GetGenericArguments().Any())
                    str = t.Name.Substring(0, str.IndexOf('`'));

                rtn += $"_{str}";
            }
        }
        return rtn;
    }

    public static TR If<T, TR>(this T input, Func<T, bool> condition, Func<T, TR> trueStatement, Func<T, TR> falseStatement)
        => condition.Invoke(input) ? trueStatement.Invoke(input) : falseStatement.Invoke(input);

    public static T If<T>(this T input, Func<T, bool> condition, Func<T, T> trueStatement, Func<T, T> falseStatement)
        => input.If<T, T>(condition, trueStatement, falseStatement);

    public static T If<T>(this T input, Func<T, bool> condition, Func<T, T> trueStatement)
        => condition.Invoke(input) ? trueStatement.Invoke(input) : input;

    public static TR If<TR>(this bool input, Func<TR> trueStatement, Func<TR> falseStatement)
        => input ? trueStatement.Invoke() : falseStatement.Invoke();

    public static void If<T>(this T input, Func<T, bool> condition, Action<T> trueStatement, Action<T> falseStatement)
    {
        if (condition.Invoke(input))
            trueStatement.Invoke(input);
        else
            falseStatement.Invoke(input);
    }

    public static void If<T>(this T input, Func<T, bool> condition, Action<T> trueStatement)
    {
        if (condition.Invoke(input))
            trueStatement.Invoke(input);
    }

    public static void If(this bool input, Action trueStatement, Action falseStatement)
    {
        if (input)
            trueStatement.Invoke();
        else
            falseStatement.Invoke();
    }

    public static void If(this bool input, Action trueStatement)
    {
        if (input)
            trueStatement.Invoke();
    }

    public static SwitchCase<T, TR> Switch<T, TR>(this T input) => new(input);

    public static SwitchCase<T, TR> Case<T, TR>(this SwitchCase<T, TR> switchCase, Func<T, bool> condition, Func<T, TR> executionStatment)
    {
        if (switchCase.output is null && condition.Invoke(switchCase.input))
            switchCase.Set(executionStatment(switchCase.input));

        return switchCase;
    }

    public static TR Result<T, TR>(this SwitchCase<T, TR> switchCase) => switchCase.output!;

    public static SwitchCase<T, T> Switch<T>(this T input) => input.Switch<T, T>();

    public static SwitchCase<T, T> Case<T>(this SwitchCase<T, T> switchCase, Func<T, bool> condition, Func<T, T> executionStatment)
        => switchCase.Case<T, T>(condition, executionStatment);

    public static T Result<T>(this SwitchCase<T, T> switchCase) => switchCase.Result<T, T>();
}