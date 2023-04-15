using BPMS.Domain.Common.Enums;
using BPMS.Infrastructure.MainHelpers;
using BPMS.Infrastructure.Services.SMS;

namespace BPMS.Application.Services.SMS;

public class ParsaSMSProvider : ISMSProvider
{
    private readonly ISmsLogService _smsLogService;
    private readonly ISmsProviderConfigService _smsProviderConfigService;

    public string Username { get; set; }
    public string Password { get; set; }
    public string FirstFrom { get; set; }
    public string SecondFrom { get; set; }
    public string ApiKey { get; set; }
    public string ProviderNumber { get; set; }
    public ParsaSMSProvider(ISmsProviderConfigService smsProviderConfigService, ISmsLogService smsLogService)
    {
        this._smsLogService = smsLogService;
        this._smsProviderConfigService = smsProviderConfigService;
    }
    public bool Send(string phoneNumber, string text)
    {
        text += "\n لغو پیامک : 11";
        bool serviceResult = false;

        var smsClientSettings = _smsProviderConfigService.GetActiveProviderConfig();

        if (smsClientSettings == null)
        {
            return false;
        }

        Username = smsClientSettings.UserName;
        Password = smsClientSettings.Password;
        ApiKey = smsClientSettings.ApiKey;
        ProviderNumber = smsClientSettings.ProviderNumber;

        string from;

        if (string.IsNullOrEmpty(ProviderNumber))
        {
            from = string.IsNullOrEmpty(FirstFrom) ? SecondFrom : FirstFrom;
        }
        else
        {
            from = ProviderNumber;
        }

        string uri = $"http://parsasms.com/tools/urlservice/send/?username={Username}&password={Password}&from={from}&to={phoneNumber}&message={text}";

        var httpClient = new HttpClient();
        var content = httpClient.GetStringAsync(uri);
        try
        {
            var result = content.Result;

            if (long.Parse(result) > 21)
            {
                serviceResult = true;
                _smsLogService.AddSmsLog(ProviderNumber, phoneNumber, text, true, "پیام با موفقیت ارسال شد.", SmsSenderType.webservice);
            }
            else
            {
                var description = HandleParsaSmsResultMessage(result);
                _smsLogService.AddSmsLog(ProviderNumber, phoneNumber, text, false, description, SmsSenderType.webservice);
            }
        }
        catch (Exception ex)
        {
            _smsLogService.AddSmsLog(ProviderNumber, phoneNumber, text, false, ex.Message, SmsSenderType.webservice);

            var exceptionHelper = new CustomExceptionHandler();
            exceptionHelper.HandleException(ex, "Error from message service");

            return false;
        }

        return serviceResult;
    }

    private string HandleParsaSmsResultMessage(string result)
    {
        string message;

        switch (result)
        {
            case "-1":
                message = "امکان ارسال پیامک به دلیل اشتباه بودن شماره وجود ندارد ";
                break;
            case "1":
                message = "نام کاربری یا رمز عبور معتبر نمی باشد ";
                break;
            case "2":
                message = "آرایه ها خالی می باشد. ";
                break;
            case "3":
                message = "طول آرایه بیشتر از 100 می باشد . ";
                break;
            case "4":
                message = "طول آرایه ی فرستنده و گیرنده و متن پیام با یکدیگر تطابق ندارد .";
                break;
            case "5":
                message = "امکان گرفتن پیام جدید وجود ندارد .";
                break;
            case "6":
                message = "حساب کاربری غیر فعال می باشد.";
                break;
            case "7":
                message = "امکان دسترسی به خط مورد نظر وجود ندارد . ";
                break;
            case "8":
                //شماره گیرنده نامعتبر است
                message = " امکان ارسال پیامک به این شماره وجود ندارد. لطفاً شماره را مجدد بررسی و یا برای اطمینان با شماره تلفن پشتیبانی 02123067 تماس حاصل فرمایید ";
                break;
            case "9":
                //موجودی حساب کافی نیست
                message = "خطایی در ارسال پیامک به وجود آمده است لطفا با تلفن پشتیبانی 02123067 تماس حاصل نمایید.";
                break;
            case "10":
                message = "خطایی در سیستم رخ داده است . دوباره سعی کنید . ";
                break;
            case "11":
                message = "IP نامعتبر می باشد .";
                break;
            case "20":
                message = "شماره شما امکان دریافت پیامک را ندارد. لطفاً با تلفن پشتیبانی 02123067 تماس حاصل نمایید.";
                break;
            case "21":
                message = "خطایی در ارسال پیامک به وجود آمده است لطفا با تلفن پشتیبانی 02123067 تماس حاصل نمایید.";
                break;
            default:
                message = "خطایی در سیستم رخ داده است . لطفاً مجدداً تلاش نمایید . ";
                break;
        }

        return message;
    }
}