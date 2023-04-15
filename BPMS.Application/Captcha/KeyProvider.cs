using System.Text;

namespace BPMS.Application.Captcha;

public class KeyProvider
{
    const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
    const string key = "mYq3s6v9y$B&E)H@McQfTjWnZr4u7w!z";
    const int maxIvLength = 16;

    public static byte[] GetAesKey() => Encoding.UTF8.GetBytes(key);

    public static string CreateNewSalt()
    {
        Random random = new();
        return new string(Enumerable.Repeat(chars, maxIvLength).Select(s => s[random.Next(s.Length)]).ToArray());
    }
}