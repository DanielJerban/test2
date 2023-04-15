namespace BPMS.Application.Services.EventManager;

public sealed class EventManagerFileContent
{
    public string Info { get; set; }
    public string InfoEms { get; set; }

    public bool IsEmsOK()
    {
        bool answer = false;

        try
        {
            if (string.IsNullOrEmpty(InfoEms) || string.IsNullOrEmpty(Info))
                return answer;

            bool signIsValid = CheckEms(Info, InfoEms);

            answer = signIsValid;
        }
        catch
        {

        }

        return answer;
    }

    private bool CheckEms(string orginalText, string emzText)
    {
        string publicString = @"MIIBITANBgkqhkiG9w0BAQEFAAOCAQ4AMIIBCQKCAQBsrxiSb/8jCcQG9gQxgdAn
DNgmGSg5SLz0/1RcBRMCb1peQUDZx89XSbpOvEt4+5+ju1W9+82UWhPRARqq2gRn
p856CDWxhQ2PXeSpgfpzPRvzz6GKtY5MckdnTn3grrk2YHu1zB/ZvKLyGC1/0FLa
PO+vreW8Y+XTlZAtqOigpyH6+XfqUMgpfxr5MkRuOb5LFVIalripWA2kbJgOcT+E
adHYve83f+2MN5zUAlKTvER/TWNRRH5WEL5kp3vEXRqqjeiB2GPDkXrYpW3b6Yn1
pt3x50mInPhBDzOua/SAuqgL7Wea/rLoZoisWN1l8JlDWoaDZihUiwIvjTZ7/Uqz
AgMBAAE=";


        var keyprovider = new System.Security.Cryptography.RSACryptoServiceProvider();

        byte[] seqOid = { 0x30, 0x0D, 0x06, 0x09, 0x2A, 0x86, 0x48, 0x86, 0xF7, 0x0D, 0x01, 0x01, 0x01, 0x05, 0x00 };
        byte[] seq = new byte[15];

        var x509Key = Convert.FromBase64String(publicString);

        using (MemoryStream mem = new MemoryStream(x509Key))
        {
            using (BinaryReader binr = new BinaryReader(mem))
            {
                byte bt = 0;
                ushort twobytes = 0;

                twobytes = binr.ReadUInt16();
                if (twobytes == 0x8130)
                    binr.ReadByte();
                else if (twobytes == 0x8230)
                    binr.ReadInt16();
                else
                    return false;

                seq = binr.ReadBytes(15);
                if (!CompareBytearrays(seq, seqOid))
                    return false;

                twobytes = binr.ReadUInt16();
                if (twobytes == 0x8103)
                    binr.ReadByte();
                else if (twobytes == 0x8203)
                    binr.ReadInt16();
                else
                    return false;

                bt = binr.ReadByte();
                if (bt != 0x00)
                    return false;

                twobytes = binr.ReadUInt16();
                if (twobytes == 0x8130)
                    binr.ReadByte();
                else if (twobytes == 0x8230)
                    binr.ReadInt16();
                else
                    return false;

                twobytes = binr.ReadUInt16();
                byte lowbyte = 0x00;
                byte highbyte = 0x00;

                if (twobytes == 0x8102)
                    lowbyte = binr.ReadByte();
                else if (twobytes == 0x8202)
                {
                    highbyte = binr.ReadByte();
                    lowbyte = binr.ReadByte();
                }
                else
                    return false;
                byte[] modint = { lowbyte, highbyte, 0x00, 0x00 };
                int modsize = BitConverter.ToInt32(modint, 0);

                int firstbyte = binr.PeekChar();
                if (firstbyte == 0x00)
                {
                    binr.ReadByte();
                    modsize -= 1;
                }

                byte[] modulus = binr.ReadBytes(modsize);

                if (binr.ReadByte() != 0x02)
                    return false;
                int expbytes = (int)binr.ReadByte();
                byte[] exponent = binr.ReadBytes(expbytes);

                var rsaKeyInfo = new System.Security.Cryptography.RSAParameters
                {
                    Modulus = modulus,
                    Exponent = exponent
                };
                keyprovider.ImportParameters(rsaKeyInfo);
            }
        }



        byte[] orginalTextInByte = Convert.FromBase64String(orginalText);
        byte[] emzTextInByte = Convert.FromBase64String(emzText);
        bool verify = keyprovider.VerifyData(orginalTextInByte, emzTextInByte, System.Security.Cryptography.HashAlgorithmName.SHA256, System.Security.Cryptography.RSASignaturePadding.Pkcs1);
        return verify;
    }

    private bool CompareBytearrays(byte[] a, byte[] b)
    {
        if (a.Length != b.Length)
            return false;
        int i = 0;
        foreach (byte c in a)
        {
            if (c != b[i])
                return false;
            i++;
        }
        return true;
    }
}