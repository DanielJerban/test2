using BPMS.Infrastructure.Services;
using System.Security.Cryptography;
using System.Text;

namespace BPMS.Application.Services;

public class PasswordService : IPasswordService
{
    public string EncryptPassword(string username, string password)
    {
        string salt = MakeSaltPassword(username, password);

        var crypt = new SHA256Managed();
        var crypto = crypt.ComputeHash(Encoding.UTF8.GetBytes(salt));

        return Convert.ToBase64String(crypto);
    }

    private string MakeSaltPassword(string username, string password)
    {
        return $"M@jid_{username}_$_{password}_D@nial";
    }
}