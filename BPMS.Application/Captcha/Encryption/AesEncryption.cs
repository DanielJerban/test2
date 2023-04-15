using System.Security.Cryptography;
using System.Text;

namespace BPMS.Application.Captcha.Encryption;

internal class AesEncryption
{
    private readonly Aes aes;
    private readonly byte[] key;

    public AesEncryption()
    {
        aes = Aes.Create();
        key = KeyProvider.GetAesKey();
    }
    public string Decrypt(string cipherText, string salt)
    {
        int ByteCount = 0;
        byte[] CipherTextBytes = Convert.FromBase64String(cipherText);
        byte[] PlainTextBytes = new byte[CipherTextBytes.Length];
        using (ICryptoTransform Decryptor = aes.CreateDecryptor(key, Encoding.UTF8.GetBytes(salt)))
        {
            using MemoryStream MemStream = new MemoryStream(CipherTextBytes);
            using CryptoStream CryptoStream = new CryptoStream(MemStream, Decryptor, CryptoStreamMode.Read);

            ByteCount = CryptoStream.Read(PlainTextBytes, 0, PlainTextBytes.Length);
            MemStream.Close();
            CryptoStream.Close();
        }

        return Encoding.UTF8.GetString(PlainTextBytes, 0, ByteCount);
    }

    public string Encrypt(string plainText, string salt)
    {
        byte[] PlainTextBytes = Encoding.UTF8.GetBytes(plainText);
        byte[]? CipherTextBytes = null;
        using (ICryptoTransform Encryptor = aes.CreateEncryptor(key, Encoding.UTF8.GetBytes(salt)))
        {
            using MemoryStream MemStream = new();
            using CryptoStream CryptoStream = new(MemStream, Encryptor, CryptoStreamMode.Write);
            CryptoStream.Write(PlainTextBytes, 0, PlainTextBytes.Length);
            CryptoStream.FlushFinalBlock();
            CipherTextBytes = MemStream.ToArray();
            MemStream.Close();
            CryptoStream.Close();
        }

        return Convert.ToBase64String(CipherTextBytes);
    }
}