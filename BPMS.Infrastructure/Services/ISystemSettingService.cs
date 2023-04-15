using BPMS.Domain.Common.Enums;
using BPMS.Domain.Entities;
using BPMS.Domain.Common.ViewModels;

namespace BPMS.Infrastructure.Services;

public interface ISystemSettingService
{
    SystemSetting CreateSystemSetting(object data, string username, SystemSettingType type);
    void ResetSystemSettingCache(SystemSettingType type);
    LdapSettingViewModel GetLDAPSetting();
    RecaptchaViewModel GetRecaptchaSetting();
    RecaptchaViewModel GetLastEnabledRecaptchaSetting();
    Stream CreateCaptcha(string offlineCaptchaMainKey);
    List<EmailConfigsViewModel> GetAllEmailSetting();
    List<SmsConfigViewModel> GetSmsSettings();
    (bool, string) ValidateCaptcha(string secretKey, string publicKey, string response);
    string GetOfflineCaptchaCache(string offlineCaptchaMainKey);
    void ResetOfflineCaptchaCache(string offlineCaptchaMainKey);
}