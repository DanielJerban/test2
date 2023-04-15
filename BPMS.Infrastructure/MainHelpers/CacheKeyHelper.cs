using BPMS.Domain.Common.Enums;

namespace BPMS.Infrastructure.MainHelpers;

public static class CacheKeyHelper
{
    public static string GetUserAccessCacheKey(Guid userId) => $"UserAccessCacheKey_{userId}";
    public static string GetRoleClaimsCacheKey(Guid roleId) => $"RoleClaimsCacheKey-{roleId}";
    public static string GetUserClaimsCacheKey(Guid userId) => $"UserClaimsCacheKey-{userId}";
    public static string GetFlowParamCacheKey(string userName) => $"FlowParamCacheKey-{userName}";
    public static string GetSecretCachekey(Guid secretKey) => $"SecretKey-{secretKey}";
    public static string GetSecureCodeTwoStepCacheKey(string mobile) => $"SecureCodeTwoStepCacheKey_{mobile}";
    public static string GetEmailSecureCodeTwoStepCacheKey(string email) => $"EmailSecureCodeTwoStepCacheKey_{email}";
    public static string GetStepsOrderCacheKey(Guid secretKey) => $"VerificationStepsOrder_{secretKey}";
    public static string GetLastSentSmsTimeCacheKey(string mobile) => $"SentSmsCodeTime_{mobile}";
    public static string GetLastEmailCodeTimeCacheKey(string email) => $"SentEmailCodeTime_{email}";
    public static string GetPostsCacheKey() => $"PostsCacheKey";
    public static string GetPrintReportCacheKey(Guid id) => $"PrintReportCacheKey-{id}";
    public static string GetStaffCacheKey(Guid userId) => $"StaffCacheKey-{userId}";
    public static string GetUserBaseInfoCacheKey(string userName) => $"GetUserBaseInfoCacheKey_{userName}";
    public static string GetUserCacheKey(string userName) => $"UserCacheKey_{userName}";
    public static string GetSystemSettingCacheKey(SystemSettingType type) => $"SystemSetting{type}"; 
    public static string GetOfflineCaptchaCacheKey(string offlineCaptchaMainKey) => $"OfflineCaptchaCacheKey_{offlineCaptchaMainKey}";
    public static string GetThirdPartyCacheKey() => "ThirdPartyCacheKey";
    public static string GetUsefulLinksCacheKey() => "UsefulLinksCacheKey";
    public static string GetSmsProviderConfigCacheKey() => "SystemSettingActiveProviderConfig";
    public static string GetMyTestCacheKey() => "MyTestCacheKey";
    public static string GetActiveConfig() => "ActiveConfig";
    public static string GetScheduleCacheKey() => "ScheduleCacheKey";
    public static string GetSystemSettingActiveProviderConfig() => "SystemSettingActiveProviderConfig";
    public static string StaffCacheKey() => "StaffCacheKey";
}