namespace BPMS.BuildingBlocks.SharedKernel.Helpers;

public static class StringHelper
{
    public static string FixKafYa(this string input)
    {
        return !string.IsNullOrEmpty(input) ? input.Replace("ك", "ک").Replace("ي", "ی") : input;
    }

    public static string NewLineIfNotEmpty(this string str)
    {
        if (!string.IsNullOrEmpty(str))
            str += Environment.NewLine;

        return str;
    }

    public static string RandomString(int length)
    {
        var chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
        var stringChars = new char[length];
        var random = new Random();

        for (int i = 0; i < stringChars.Length; i++)
            stringChars[i] = chars[random.Next(chars.Length)];

        var finalString = new string(stringChars);
        return finalString;
    }

    public static string Abbraviate(this string input, int length = 2, params string[] escapes)
    {
        if (string.IsNullOrEmpty(input) || input.Contains(' '))
            throw new ArgumentException("Invalid input string, input value must be single phrase without any space charachter");

        var temp = input;
        if (escapes?.Length > 0)
        {
            foreach (var escape in escapes)
                temp = temp.Replace(escape, "", StringComparison.InvariantCultureIgnoreCase);
        }

        if (temp.Length < length)
        {
            var temp1 = RandomString(length - temp.Length) + temp;
            while (escapes?.Contains(temp1) == true)
                temp1 = RandomString(length - temp.Length) + temp;

            temp = temp1;
        }

        return temp[..length];
    }

    public static string Ellipsis(this string input, int size = 100)
    {
        if (input.Length <= size)
            return input;

        if (size > 3)
            size -= 3;

        return $"{input[..size]}...";
    }
}