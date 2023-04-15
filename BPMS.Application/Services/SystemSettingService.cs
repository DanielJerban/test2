using BPMS.Application.Captcha;
using BPMS.Application.Captcha.Encryption;
using BPMS.Application.Captcha.Models;
using BPMS.Domain.Common.Dtos.Recaptcha;
using BPMS.Domain.Common.Enums;
using BPMS.Domain.Common.ViewModels;
using BPMS.Domain.Entities;
using BPMS.Infrastructure;
using BPMS.Infrastructure.MainHelpers;
using BPMS.Infrastructure.Services;
using Newtonsoft.Json;
using System.Dynamic;
using System.Net;

namespace BPMS.Application.Services;

public class SystemSettingService : ISystemSettingService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IUserService _userServices;
    private readonly IDistributedCacheHelper _cacheHelper;

    public SystemSettingService(IUnitOfWork unitOfWork, IDistributedCacheHelper cacheHelper, IUserService userServices)
    {
        _unitOfWork = unitOfWork;
        _cacheHelper = cacheHelper;
        _userServices = userServices;
    }

    public SystemSetting CreateSystemSetting(object data, string username, SystemSettingType type)
    {
        var user = _userServices.GetUser(username);
        string jsonData = JsonConvert.SerializeObject(data);
        var newSetting = new SystemSetting() { Data = jsonData, Type = type, CreatorUserId = user.Id };
        _unitOfWork.SystemSetting.Add(newSetting);
        ResetSystemSettingCache(type);
        ResetSystemSettingActiveProviderConfig();

        _unitOfWork.Complete();
        return newSetting;
    }

    public LdapSettingViewModel GetLDAPSetting()
    {
        var systemSetting = GetLastSetting(SystemSettingType.Ldap);
        return systemSetting == null ? null : JsonConvert.DeserializeObject<LdapSettingViewModel>(systemSetting.Data);
    }

    public RecaptchaViewModel GetLastEnabledRecaptchaSetting()
    {
        var allRecatchaSettingOrderbyDate = _unitOfWork.SystemSetting.Where(c => c.Type == SystemSettingType.Recaptcha)
            .OrderByDescending(c => c.CreatedDate);
        var last = new RecaptchaViewModel();
        foreach (var item in allRecatchaSettingOrderbyDate.ToList())
        {
            var data = JsonConvert.DeserializeObject<RecaptchaViewModel>(item.Data);
            if (data.IsRecaptchaEnable)
            {
                last = data;
                break;
            }
        }

        return last;
    }
    public RecaptchaViewModel GetRecaptchaSetting()
    {
        var systemSetting = GetLastSetting(SystemSettingType.Recaptcha);
        if (systemSetting == null) return null;

        return JsonConvert.DeserializeObject<RecaptchaViewModel>(systemSetting.Data);
    }
    public Stream CreateCaptcha(string offlineCaptchaMainKey)
    {
        (string code, string encryptedValue) New()
        {
            var encryption = new AesEncryption();
            Random random = new();
            var code = random.Next(1001, 9999).ToString();

            //encrypt code
            //generate salt for each request
            var salt = KeyProvider.CreateNewSalt();

            //generate captcha model
            dynamic captchaModel = new ExpandoObject();
            captchaModel.Captcha = encryption.Encrypt(code, salt);
            captchaModel.Salt = salt;

            //encrypt captcha model
            string encrypted = encryption.Encrypt(JsonConvert.SerializeObject(captchaModel), offlineCaptchaMainKey);

            return (code, encrypted);
        }
        var (captchaCode, encryptedValue) = New();

        // todo: add "encryptedValue" to cache 
        SetOfflineCaptchaCache(offlineCaptchaMainKey, captchaCode);
        ;
        // todo: read captcha option from web.config
        CaptchaGenerator captchaGenerator = new CaptchaGenerator(new CaptchaOptions());
        var captchaImage = captchaGenerator.GenerateImageAsStream(captchaCode);

        return captchaImage;
    }
    public (bool, string) ValidateCaptcha(string secretKey, string publicKey, string response)
    {
        var result = (false, "اطلاعات وارد شده معتبر نیست و امکان فعالسازی وجود ندارد.");
        if (string.IsNullOrEmpty(response))
            result.Item2 = "برای اعتبارسنجی ریکپچا را انتخاب کنید";
        var client = new WebClient();
        try
        {
            var reply = client.DownloadString(string.Format("https://www.google.com/recaptcha/api/siteverify?secret={0}&response={1}", secretKey, response));
            var captchaResponse = JsonConvert.DeserializeObject<CaptchaResponse>(reply);

            var res = Convert.ToBoolean(captchaResponse.Success);
            if (res)
            {
                result.Item1 = res;
                result.Item2 = "  ریکپچا معتبر است";
            }

        }
        catch (Exception e)
        {
            result.Item2 = " برای استفاده از ریکپچا نیاز به اینترت دارید";
        }

        return result;

    }

    public List<EmailConfigsViewModel> GetAllEmailSetting()
    {
        var systemSetting = GetLastSetting(SystemSettingType.Email);
        return systemSetting == null ? null : JsonConvert.DeserializeObject<List<EmailConfigsViewModel>>(systemSetting.Data);
    }

    private SystemSetting GetLastSetting(SystemSettingType type)
    {
        return _cacheHelper.GetOrSet(CacheKeyHelper.GetSystemSettingCacheKey(type), () => getLastSystemSetting(type), TimeSpan.FromDays(1));
    }

    private SystemSetting getLastSystemSetting(SystemSettingType type)
    {
        var result = _unitOfWork.SystemSetting.Where(c => c.Type == type).OrderByDescending(c => c.CreatedDate).FirstOrDefault();

        return result;
    }

    public void ResetSystemSettingCache(SystemSettingType type)
    {
        _cacheHelper.Remove(CacheKeyHelper.GetSystemSettingCacheKey(type));
    }

    public void ResetSystemSettingActiveProviderConfig()
    {
        _cacheHelper.Remove(CacheKeyHelper.GetSystemSettingActiveProviderConfig());
    }

    private void SetOfflineCaptchaCache(string offlineCaptchaMainKey, string key)
    {
        _cacheHelper.SetString(CacheKeyHelper.GetOfflineCaptchaCacheKey(offlineCaptchaMainKey), key, TimeSpan.FromMinutes(1));
    }
    public string GetOfflineCaptchaCache(string offlineCaptchaMainKey)
    {
        return _cacheHelper.GetObject<string>(CacheKeyHelper.GetOfflineCaptchaCacheKey(offlineCaptchaMainKey));
    }
    public void ResetOfflineCaptchaCache(string offlineCaptchaMainKey)
    {
        _cacheHelper.Remove(CacheKeyHelper.GetOfflineCaptchaCacheKey(offlineCaptchaMainKey));
    }

    public List<SmsConfigViewModel> GetSmsSettings()
    {
        var systemSetting = GetLastSetting(SystemSettingType.SMS);
        if (systemSetting.Data == null)
        {
            return new List<SmsConfigViewModel>();
        }

        return JsonConvert.DeserializeObject<List<SmsConfigViewModel>>(systemSetting.Data);

    }
}