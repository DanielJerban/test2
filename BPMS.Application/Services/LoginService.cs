using BPMS.Domain.Common.Dtos;
using BPMS.Domain.Common.Dtos.Recaptcha;
using BPMS.Domain.Common.Enums;
using BPMS.Domain.Entities;
using BPMS.Infrastructure;
using BPMS.Infrastructure.MainHelpers;
using BPMS.Infrastructure.Services;
using BPMS.Infrastructure.Services.Email;
using BPMS.Infrastructure.Services.SMS;
using Google.Authenticator;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System.Net;

namespace BPMS.Application.Services;

public class LoginService : ILoginService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IPasswordService _passwordService;
    private readonly ILDAPService _ldapService;
    private readonly IUserLoginOutService _userLoginOutService;
    private readonly ISystemSettingService _systemSettingService;
    private readonly IDistributedCacheHelper _cacheHelper;
    private readonly IEmailSender _emailSender;
    private readonly ISendingSmsService _sendingSmsService;


    private readonly string _bpmsBaseUrl;
    public LoginService(IUnitOfWork unitOfWork,
        IPasswordService passwordService,
        ILDAPService lDAPService,
        IUserLoginOutService userLoginOutService,
        ISystemSettingService systemSettingService,
        IDistributedCacheHelper cacheHelper,
        IEmailSender emailSender, IConfiguration configuration, ISendingSmsService sendingSmsService)
    {
        _unitOfWork = unitOfWork;
        _passwordService = passwordService;
        _ldapService = lDAPService;
        _userLoginOutService = userLoginOutService;
        _systemSettingService = systemSettingService;
        _cacheHelper = cacheHelper;
        _emailSender = emailSender;
        _sendingSmsService = sendingSmsService;
        _bpmsBaseUrl = configuration["Common:BpmsBaseUrl"];
    }

    public bool CheckUsernameAndPassword(string username, string password, out User actualUser, out string Message)
    {
        bool result = false;
        actualUser = null;
        Message = "";
        CheckUserIsValidByUsernamePassword(username, password, out bool success, out string errorMessage, out var user);
        if (success)
        {
            actualUser = user;
            result = true;
        }

        Message = errorMessage;
        return result;
    }

    public bool CheckUserIsValidByLDAPConnection(string username, string password, out string Message, out User actualUser)
    {
        bool result = false;
        Message = "";
        actualUser = null;

        string ldapDomain = "";
        string ldapUsername = username;

        if (username.Contains(@"\"))
        {
            var split = username.Split('\\');
            ldapDomain = split[0];
            ldapUsername = split[1];
        }

        var usersWithDomains = _unitOfWork.Users.Where(p => p.LDAPUserName == ldapUsername);
        if (!string.IsNullOrEmpty(ldapDomain))
        {
            usersWithDomains = usersWithDomains.Where(p => p.LDAPDomainName == ldapDomain);
        }

        var user = usersWithDomains.FirstOrDefault();

        if (user == null)
        {
            Message = "نام کاربری یا کلمه عبور صحیح نیست.";
            return result;
        }

        if (user.Staff.EngType.Code == 2)
        {
            Message = "شما اجازه ورود به سایت را ندارید.";
            return result;
        }

        if (!user.IsActive)
        {
            Message = "حساب کاربری شما فعال نشده است.";
            return result;
        }

        bool isUserValid = _ldapService.ValidateUserInLDAP(ldapUsername, password, ldapDomain);

        if (!isUserValid)
        {
            Message = "نام کاربری یا کلمه عبور صحیح نیست.";
            return result;
        }

        result = true;
        actualUser = user;
        return result;
    }

    public List<VerificationSteps> GetUserVerificationStepsByOrder(User user, Guid secretKey)
    {
        var steps = _cacheHelper.GetOrSet(CacheKeyHelper.GetStepsOrderCacheKey(secretKey),
            () => getUserVerificationStepsByOrder(user), TimeSpan.FromMinutes(7));
        return steps;
    }
    public Guid GetSecurityKeyAndSetusernameCache(string userName)
    {
        var key = GenerateSecretKey();
        SetUsernameToCache(key, userName);
        return key;
    }

    public string SendSmsCode(string mobile)
    {
        var secureCode = PasswordGenerator.Generate(4, 6);
        string message = $@" {secureCode}: کد تایید @ {_bpmsBaseUrl}";
        message = message.Replace("@", System.Environment.NewLine);
        _sendingSmsService.SendSms(mobile, message);
        SetSecureCodeToCache(secureCode, mobile);
        return secureCode;

    }
    public string SendEmailCode(string email)
    {
        var secureCode = PasswordGenerator.Generate(4, 6);
        string message = $@" {secureCode}: کد تایید لاگین با ایمیل قابل استفاده در سیستم";
        message = message.Replace("@", System.Environment.NewLine);
        var emails = new List<string> { email };
        _emailSender.Send(emails, new MessageContent
        {
            Subject = "کد تایید لاگین با ایمیل",
            Body = message
        });
        SetEmailSecureCodeToCache(secureCode, email);
        return secureCode;
    }

    public bool ValidateEmailCode(TwoStepVerificationEmailInputDto model) =>
        GetEmailSecureCode(model?.Email) == model?.EmailCode;

    public bool ValidateGoogleAuthCode(TwoStepVerificationGoogleAuthInputDto model)
    {
        var result = false;
        var username = GetUsername(model.SecretKey);

        if (username != null)
        {
            var user = _unitOfWork.Users.Where(i => i.UserName == username).FirstOrDefault();
            TwoFactorAuthenticator tfa = new TwoFactorAuthenticator();
            result = tfa.ValidateTwoFactorPIN(user.GoogleAuthKey, model.GoogleAuthCode);
        }

        return result;
    }

    public bool ValidateSmsCode(TowStepVerificationSMSInputDTO model) => GetSecureCode(model?.Mobile) == model?.SMSCode;


    // Todo: Uncomment later 
    public void Login(string key, bool rememberMe, UserSystemDataDto model)
    {
        //var secretKey = Guid.Parse(key);
        //var username = GetUsername(secretKey);
        //var result = new Result();
        //var sys = _unitOfWork.LookUps.Single(l => l.Code == 1 && l.Type == "System");
        //if (sys.IsActive)
        //{
        //    result.Message = sys.Title;
        //    result.Success = false;
        //    throw new ArgumentException(sys.Title, "info");
        //}

        //var user = _unitOfWork.Users.Where(i => i.UserName == username).FirstOrDefault();
        //FormsAuthentication.SetAuthCookie(username, rememberMe);
        //LogSuccessLogin(user.Id, user.UserName, model);

        //ResetAllLoginProcessCacheKeis(secretKey, user);
    }

    public void LogFailureLogin(string username, UserSystemDataDto model)
    {
        LoginLogoutLog(Guid.Empty, username, 3, model);
    }

    public void LogSuccessLogin(Guid userId, string username, UserSystemDataDto model)
    {
        LoginLogoutLog(userId, username, 1, model);
    }

    public void LogSuccessLogout(Guid userId, string username, UserSystemDataDto model)
    {
        LoginLogoutLog(userId, username, 2, model);
    }


    public bool ResendSmsCode(TwoStepVerDto model)
    {
        var result = false;
        var username = GetUsername(model.SecurityKey);
        if (!string.IsNullOrEmpty(username))
        {
            var user = _unitOfWork.Users.Where(i => i.UserName == username).FirstOrDefault();
            var mobile = user.Staff.PhoneNumber;
            ResetSecureCodeCache(mobile);
            SendSmsCode(mobile);
            result = true;
        }

        return result;
    }

    public bool ResendEmailCode(Guid securityKey)
    {
        var result = false;
        var username = GetUsername(securityKey);
        if (!string.IsNullOrEmpty(username))
        {
            var user = _unitOfWork.Users.Where(i => i.UserName == username).FirstOrDefault();
            var email = user.Staff.Email;
            ResetEmailSecureCodeCache(email);
            SendEmailCode(email);
            result = true;
        }

        return result;
    }
    public string GetUsername(Guid secretKey)
    {
        return _cacheHelper.GetString(CacheKeyHelper.GetSecretCachekey(secretKey));
    }


    public string GetSecureCode(string mobile)
    {
        return _cacheHelper.GetObject<string>(GetSecureCodeTwoStepCacheKey(mobile));
    }
    public string GetEmailSecureCode(string email)
    {
        return _cacheHelper.GetObject<string>(GetEmailSecureCodeTwoStepCacheKey(email));
    }
    public void ResetSecureCodeCache(string mobile)
    {
        ResetSecureCodeCacheKey(mobile);
        ResetLastSentSmsTimeCacheKey(mobile);
    }
    public void ResetEmailSecureCodeCache(string email)
    {
        ResetEmailSecureCodeCacheKey(email);
        ResetLastEmailCodeTimeCacheKey(email);
    }
    public void ResetSecretKeyCache(Guid secretKey)
    {
        ResetCache(secretKey);
    }



    public int GetLastEmailCodeRemainingTime(string email)
    {
        var remainingTime = 0;
        DateTime lastSentMessageTime = GetLastEmailCodeTime(email);

        if (lastSentMessageTime != default)
        {
            DateTime endTime = lastSentMessageTime.AddSeconds(120);
            var remaining = (int)(endTime - DateTime.Now).TotalSeconds;
            if (remaining > 0)
                remainingTime = remaining;
        }

        return remainingTime;
    }


    public int GetLastSentSmsRemainingTime(string mobile)
    {
        var remainingTime = 0;
        DateTime lastSentMessageTime = GetLastSentSmsTime(mobile);

        if (lastSentMessageTime != default)
        {
            DateTime endTime = lastSentMessageTime.AddSeconds(60);
            var remaining = (int)(endTime - DateTime.Now).TotalSeconds;
            if (remaining > 0)
                remainingTime = remaining;
        }

        return remainingTime;
    }

    public User GetUserBySecretKey(Guid secretKey)
    {
        var username = GetUsername(secretKey);

        var user = _unitOfWork.Users.Where(p => p.UserName == username).FirstOrDefault();
        if (user == null)
        {
            string ldapDomain = "";
            string ldapUsername = username;

            if (username.Contains(@"\"))
            {
                var split = username.Split('\\');
                ldapDomain = split[0];
                ldapUsername = split[1];
            }

            var usersWithDomains = _unitOfWork.Users.Where(p => p.LDAPUserName == ldapUsername);
            if (!string.IsNullOrEmpty(ldapDomain))
            {
                usersWithDomains = usersWithDomains.Where(p => p.LDAPDomainName == ldapDomain);
            }
            user = usersWithDomains.FirstOrDefault();
        }

        return user;
    }

    public (bool, string) ValidateCaptcha(string response)
    {
        string secret = _systemSettingService.GetRecaptchaSetting()?.RecaptchaSecretKey;
        var result = (false, "ریکپچا نا معتبر است");
        if (string.IsNullOrEmpty(response))
            result.Item2 = "برای اعتبارسنجی ریکپچا را انتخاب کنید";
        var client = new WebClient();
        try
        {
            var reply = client.DownloadString(string.Format("https://www.google.com/recaptcha/api/siteverify?secret={0}&response={1}", secret, response));
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

    //private string GetCachekey(Guid secretKey) => $"SecretKey-{secretKey}";

    private string GetSecureCodeTwoStepCacheKey(string mobile) => $"SecureCodeTwoStepCacheKey_{mobile}";
    private string GetEmailSecureCodeTwoStepCacheKey(string email) => $"EmailSecureCodeTwoStepCacheKey_{email}";
    private void ResetCache(Guid secretKey) => _cacheHelper.Remove(CacheKeyHelper.GetSecretCachekey(secretKey));
    private void ResetEmailSecureCodeCacheKey(string email) => _cacheHelper.Remove(GetEmailSecureCodeTwoStepCacheKey(email));
    private void ResetSecureCodeCacheKey(string mobile) => _cacheHelper.Remove(GetSecureCodeTwoStepCacheKey(mobile));
    private void LoginLogoutLog(Guid userId, string userName, int code, UserSystemDataDto model)
    {
        var date = int.Parse(DateTime.Now.ToString("yyyyMMdd"));
        var time = DateTime.Now.ToString("HHmm");
        var lookup = _unitOfWork.LookUps.Where(l => l.Code == code && l.Type == "UserLoginOutType").FirstOrDefault();
        if (model.IP == "::1")
            model.IP = "127.0.0.1";

        if (model.HostName == "::1")
            model.HostName = "127.0.0.1";

        if (lookup != null)
        {
            var userLoginOut = new UserLoginOutDto()
            {
                UserId = userId,
                Date = date,
                Time = time,
                LoginOutTypeId = lookup.Id,
                Ip = model.IP,
                BrowserName = model.BrowserName + " - " + model.BrowserVersion,
                MachineName = model.HostName,
                UserName = userName
            };
            _userLoginOutService.AddUserLoginOutLog(userLoginOut);
        }
    }
    private void SetUsernameToCache(Guid secretKey, string username)
    {
        _cacheHelper.SetString(CacheKeyHelper.GetSecretCachekey(secretKey), username, TimeSpan.FromMinutes(10));

    }
    private Guid GenerateSecretKey()
    {
        return Guid.NewGuid();
    }

    private void CheckUserIsValidByUsernamePassword(string username, string password, out bool success, out string errorMessage, out User selectedUser)
    {
        success = false;
        errorMessage = "";
        selectedUser = null;

        var user = _unitOfWork.Users.Where(p => p.UserName == username).FirstOrDefault();
        if (user != null)
        {
            if (!user.IsActive)
            {
                errorMessage = "حساب کاربری شما فعال نشده است.";
                return;
            }

            var encPass = _passwordService.EncryptPassword(username, password);
            if (user.Password != encPass)
            {
                errorMessage = "نام کاربری یا کلمه عبور صحیح نیست.";
                return;
            }

            if (user.Staff.EngType.Code == 2)
            {
                errorMessage = "شما اجازه ورود به سایت را ندارید.";
                return;
            }
        }
        else
        {
            errorMessage = "نام کاربری یا کلمه عبور صحیح نیست.";
            return;
        }

        success = true;
        selectedUser = user;
    }




    private List<VerificationSteps> getUserVerificationStepsByOrder(User currentUser)
    {
        var steps = new List<VerificationSteps>();
        if (currentUser.TwoStepVerificationByGoogleAuthenticator)
            steps.Add(VerificationSteps.VerificationByGoogleAuthenticator);
        if (currentUser.TwoStepVerification)
            steps.Add(VerificationSteps.VerificationBySms);
        if (currentUser.TwoStepVerificationByEmail)
            steps.Add(VerificationSteps.VerificationByEmail);

        steps.Add(VerificationSteps.VerificationByUsernameAndPassword);
        return steps;
    }

    private void SetStepsToCache(List<VerificationSteps> steps, Guid secretkey)
    {
        _cacheHelper.SetObject(CacheKeyHelper.GetStepsOrderCacheKey(secretkey), steps, TimeSpan.FromMinutes(7));
    }
    private List<VerificationSteps> GetStepsFromCache(Guid secretkey)
    {
        return _cacheHelper.GetObject<List<VerificationSteps>>(CacheKeyHelper.GetStepsOrderCacheKey(secretkey));
    }

    private void ResetStepsCacheKey(Guid secretkey) => _cacheHelper.Remove(CacheKeyHelper.GetStepsOrderCacheKey(secretkey));
    private void SetSecureCodeToCache(string secureCode, string mobile)
    {
        _cacheHelper.SetObject(GetSecureCodeTwoStepCacheKey(mobile), secureCode, TimeSpan.FromSeconds(120));
        SetLastSentSmsTime(mobile);

    }

    private void SetLastSentSmsTime(string mobile)
    {
        _cacheHelper.SetObject(CacheKeyHelper.GetLastSentSmsTimeCacheKey(mobile), DateTime.Now, TimeSpan.FromSeconds(63));
    }
    private DateTime GetLastSentSmsTime(string mobile)
    {
        return _cacheHelper.GetObject<DateTime>(CacheKeyHelper.GetLastSentSmsTimeCacheKey(mobile));
    }
    private void ResetLastSentSmsTimeCacheKey(string mobile) => _cacheHelper.Remove(CacheKeyHelper.GetLastSentSmsTimeCacheKey(mobile));

    private void SetLastEmailCodeTime(string email)
    {
        _cacheHelper.SetObject(CacheKeyHelper.GetLastEmailCodeTimeCacheKey(email), DateTime.Now, TimeSpan.FromSeconds(123));
    }
    private DateTime GetLastEmailCodeTime(string email)
    {
        return _cacheHelper.GetObject<DateTime>(CacheKeyHelper.GetLastEmailCodeTimeCacheKey(email));
    }

    private void ResetLastEmailCodeTimeCacheKey(string email) => _cacheHelper.Remove(CacheKeyHelper.GetLastEmailCodeTimeCacheKey(email));
    private void SetEmailSecureCodeToCache(string secureCode, string email)
    {
        _cacheHelper.SetObject(GetEmailSecureCodeTwoStepCacheKey(email), secureCode, TimeSpan.FromSeconds(180));
        SetLastEmailCodeTime(email);
    }
    private void ResetAllLoginProcessCacheKeis(Guid secretKey, User user)
    {
        var email = user.Staff.Email;
        var mobile = user.Staff.PhoneNumber;
        ResetCache(secretKey);
        ResetStepsCacheKey(secretKey);
        ResetLastEmailCodeTimeCacheKey(email);
        ResetLastSentSmsTimeCacheKey(mobile);
        ResetSecureCodeCache(mobile);
        ResetEmailSecureCodeCache(email);
    }

}