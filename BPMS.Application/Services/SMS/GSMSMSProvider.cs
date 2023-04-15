using BPMS.Infrastructure.Services.SMS;

namespace BPMS.Application.Services.SMS;

public class GSMSMSProvider : ISMSProvider
{
    //public GSMSMSProvider(ISmsProviderConfigService smsProviderConfigService, ISmsLogService smsLogService)
    //{
    //    this.smsLogService = smsLogService;
    //    this._smsProviderConfigService = smsProviderConfigService;
    //}

    //public string Port { get; set; }
    //public string PortRate { get; set; }

    //private static object locker = new object();
    //private GsmCommMain comm;
    //private readonly ISmsLogService smsLogService;
    //private readonly ISmsProviderConfigService _smsProviderConfigService;

    public bool Send(string phoneNumber, string text)
    {
        bool serviceResult = false;
        //lock (locker)
        //{
        //    try
        //    {
        //        var smsClientSettings = _smsProviderConfigService.GetActiveProviderConfig();

        //        Port = smsClientSettings.GsmPort;
        //        PortRate = smsClientSettings.GsmPortRate;
        //        if (Connect(Port, PortRate))
        //        {
        //            serviceResult = SendSms(phoneNumber, text, out string result);
        //            ClosePort();
        //            smsLogService.AddSmsLog(null, phoneNumber, text, true, null, SmsSenderType.gsm);
        //        }
        //        throw new ArgumentException();
        //    }
        //    catch (Exception ex)
        //    {
        //        smsLogService.AddSmsLog(null, phoneNumber, text, false, ex.Message, SmsSenderType.gsm);
        //    }
        //}

        return serviceResult;
    }

    //private bool Connect(string port, string rate)
    //{
    //    int boudrate;
    //    int.TryParse(rate, out boudrate);
    //    try
    //    {
    //        this.comm = null;
    //        comm = new GsmCommMain(port, boudrate, 300);
    //        bool retry;
    //        do
    //        {
    //            retry = false;
    //            try
    //            {
    //                comm.Open();
    //            }
    //            catch
    //            {

    //                return false;
    //            }

    //        } while (retry);

    //    }
    //    catch
    //    {
    //        return false;

    //    }
    //    return true;
    //}
    //private bool SendSms(string phoneNumber, string text, out string result)
    //{
    //    result = "اشکال در ارسال";
    //    try
    //    {

    //        if (comm.IsConnected())
    //        {
    //            bool smartUniCode = true;
    //            OutgoingSmsPdu[] pdus = CreateContactMessage(text, phoneNumber, smartUniCode);
    //            if (pdus != null)
    //                SendMultiple(pdus);
    //            result = "ارسال شد";
    //            return true;

    //        }
    //        else
    //        {
    //            return false;
    //        }
    //    }
    //    catch
    //    {

    //        return false;
    //    }
    //}

    //private OutgoingSmsPdu[] CreateContactMessage(string message, string number, bool uniCode)
    //{
    //    OutgoingSmsPdu[] pdus = null;
    //    try
    //    {
    //        if (!uniCode)
    //        {
    //            pdus = SmartMessageFactory.CreateConcatTextMessage(message, number);
    //        }
    //        else
    //        {
    //            pdus = SmartMessageFactory.CreateConcatTextMessage(message, true, number);
    //        }

    //    }
    //    catch
    //    {

    //        return null;
    //    }

    //    if (pdus.Length == 0)
    //    {
    //        return null;
    //    }

    //    return pdus;

    //}
    //private void SendMultiple(OutgoingSmsPdu[] pdus)
    //{
    //    try
    //    {
    //        int i = 0;
    //        foreach (OutgoingSmsPdu pdu in pdus)
    //        {
    //            i++;
    //            comm.SendMessage(pdu, false);

    //        }

    //    }
    //    catch
    //    {


    //    }

    //}
    //private bool ClosePort()
    //{
    //    try
    //    {
    //        comm.Close();
    //        return true;
    //    }
    //    catch
    //    {
    //        return false;

    //    }

    //}
}