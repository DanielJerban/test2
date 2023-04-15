using Newtonsoft.Json;

namespace BPMS.Application.Services.EventManager;

public sealed class EventReaderService
{
    public EventManagerInfoAggregate ReadInfo(string eventContentText, out bool eventIsOK)
    {
        eventIsOK = false;

        EventManagerInfoAggregate answer = null;

        try
        {
            if (string.IsNullOrEmpty(eventContentText))
                return answer;

            string eventContentJson = MakeTextReady(eventContentText, EventString3, EventString4);

            var eventFileContent = JsonConvert.DeserializeObject<EventManagerFileContent>(eventContentJson);

            if(eventFileContent == null || !eventFileContent.IsEmsOK())
                return answer;

            string eventInfoJson = MakeTextReady(eventFileContent.Info, EventString1, EventString2);

            answer = JsonConvert.DeserializeObject<EventManagerInfoAggregate>(eventInfoJson);

            if (answer == null || !answer.IsOK())
                return answer;

            eventIsOK = true;
        }
        catch
        {
            eventIsOK = false;
        }

        return answer;
    }

    public EventManagerInfoAggregate ReadEventFile(out bool eventIsValid, string eventDirPath = "")
    {
        eventIsValid = false;

        EventManagerInfoAggregate answer = null;

        try
        {
            string fileName = "BPMS.License";

            if (!string.IsNullOrEmpty(eventDirPath))
                eventDirPath = eventDirPath + "\\";

            string eventFilePath = $"{eventDirPath}{fileName}";

            if (!File.Exists(eventFilePath))
                return answer;

            string licenseFileTextContent = File.ReadAllText(eventFilePath);

            answer = ReadInfo(licenseFileTextContent, out eventIsValid);
        }
        catch
        {

        }
        return answer;
    }

        

    private string MakeTextReady(string textToMake, string eventString1, string eventString2)
    {
        byte[] Key = Convert.FromBase64String(eventString1);
        byte[] IV = Convert.FromBase64String(eventString2);
        byte[] cipherText = Convert.FromBase64String(textToMake);

        string plaintext = null;

        using (System.Security.Cryptography.Rijndael rijAlg = System.Security.Cryptography.Rijndael.Create())
        {
            rijAlg.BlockSize = 128;

            rijAlg.Key = Key;
            rijAlg.IV = IV;

            System.Security.Cryptography.ICryptoTransform decryptor = rijAlg.CreateDecryptor(rijAlg.Key, rijAlg.IV);

            using (MemoryStream msDecrypt = new MemoryStream(cipherText))
            {
                using (var csDecrypt = new System.Security.Cryptography.CryptoStream(msDecrypt, decryptor, System.Security.Cryptography.CryptoStreamMode.Read))
                {
                    using (StreamReader srDecrypt = new StreamReader(csDecrypt))
                    {
                        plaintext = srDecrypt.ReadToEnd();
                    }
                }
            }

        }

        return plaintext;
    }

    private string EventString1 => "Wlp7u32fwgC8fXRZl2biai1nGoYrXbvipMe290dQKp8=";
    private string EventString2 => "c0ch3pIC5gC8Mz1rzKrzSQ==";

    private string EventString3 => "tt051RCysB7Lq7BKwgl/jfbahAi5NRfXCtSewFGk4EU=";
    private string EventString4 => "kelWOzheCZ2JweVcqGDSAw==";
}