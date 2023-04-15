using BPMS.Application.Services.EventManager;
using Newtonsoft.Json;
using System.Net;
using System.Security.Cryptography;

namespace BPMS.Application.Services.SupportCenter;

public class QueekLoginService : IQueekLoginService
{
    public string QueekToSupportCenter()
    {
        string answerUrl = "";

        try
        {
            string key = "5XpRqhgEE7mmFV06rFMP12AO3mra6GT1y5GiXLoSLa4=";
            string iv = "iF46jVkYsHdbx9Is25laig==";

            string textKey = $"{getSecondsFromBaseDateTimeToNowForQueekLogin()}##{EventDataStoreService.EventInfos.BaseInfo.UniqueId}";
            string encryptedText = encryptText(textKey, key, iv);

            var result = sendLoginRequest(new GetSecretKeyInputDTO() { RequestKey = encryptedText });
            if (result == null || !result.Success)
                return answerUrl;

            answerUrl = $"{SupportAddressManager.GetSupportCenterUrl()}/Account/QueekLogin?secretKey={result.SecretKey}";
        }
        catch { }

        return answerUrl;
    }

    private string encryptText(string plainText, string rijndaelKey, string rijndaelIV)
    {
        try
        {
            byte[] Key = Convert.FromBase64String(rijndaelKey);
            byte[] IV = Convert.FromBase64String(rijndaelIV);

            byte[] encrypted;
            using (Rijndael rijAlg = Rijndael.Create())
            {
                rijAlg.BlockSize = 128;

                rijAlg.Key = Key;
                rijAlg.IV = IV;

                ICryptoTransform encryptor = rijAlg.CreateEncryptor(rijAlg.Key, rijAlg.IV);

                using (MemoryStream msEncrypt = new MemoryStream())
                {
                    using (CryptoStream csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                    {
                        using (StreamWriter swEncrypt = new StreamWriter(csEncrypt))
                        {
                            swEncrypt.Write(plainText);
                        }
                        encrypted = msEncrypt.ToArray();
                    }
                }
            }

            string encryptedtext = Convert.ToBase64String(encrypted);
            return encryptedtext;
        }
        catch
        {
            return "";
        }
    }

    private long getSecondsFromBaseDateTimeToNowForQueekLogin()
    {
        DateTime baseDate = new DateTime(1993, 6, 12, 8, 0, 0);
        DateTime nowDate = DateTime.Now;
        var defferentTime = nowDate - baseDate;
        return (long)defferentTime.TotalSeconds;
    }

    private GetSecretKeyOutputDTO sendLoginRequest(GetSecretKeyInputDTO inputDTO)
    {
        GetSecretKeyOutputDTO answer = null;

        try
        {
            string requestUrl = $"{SupportAddressManager.GetSupportCenterUrl()}/api/v1/QueekLogin/GetSecretKey";
            string requestBody = JsonConvert.SerializeObject(inputDTO);

            var httpWebRequest = (HttpWebRequest)WebRequest.Create(requestUrl);
            httpWebRequest.ContentType = "application/json";
            httpWebRequest.Method = "POST";

            using (var streamWriter = new StreamWriter(httpWebRequest.GetRequestStream()))
            {
                streamWriter.Write(requestBody);
            }

            var httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();
            using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
            {
                string result = streamReader.ReadToEnd();
                if (!string.IsNullOrEmpty(result))
                {
                    answer = JsonConvert.DeserializeObject<GetSecretKeyOutputDTO>(result);
                }
            }
        }
        catch { }

        return answer;
    }
}

public class GetSecretKeyInputDTO
{
    public string RequestKey { get; set; }
}

public class GetSecretKeyOutputDTO
{
    public bool Success { get; set; }
    public string SecretKey { get; set; }
}