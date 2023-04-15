using BPMS.Domain.Common.Dtos;
using BPMS.Domain.Common.Enums;
using BPMS.Domain.Entities;

namespace BPMS.Infrastructure.Services;

public interface ILoginService
{
    bool CheckUsernameAndPassword(string username, string password, out User actualUser, out string Message);
    bool CheckUserIsValidByLDAPConnection(string username, string password, out string Message, out User selectedUser);
    Guid GetSecurityKeyAndSetusernameCache(string userName);
    string GetUsername(Guid secretKey);
    void ResetSecureCodeCache(string mobile);
    void ResetSecretKeyCache(Guid secretKey);
    string SendSmsCode(string mobile);
    string SendEmailCode(string email);
    bool ValidateSmsCode(TowStepVerificationSMSInputDTO model);
    bool ValidateEmailCode(TwoStepVerificationEmailInputDto model);
    void Login(string Key, bool rememberMe, UserSystemDataDto model);
    User GetUserBySecretKey(Guid secretKey);
    bool ResendSmsCode(TwoStepVerDto model);
    bool ResendEmailCode(Guid securityKey);
    List<VerificationSteps> GetUserVerificationStepsByOrder(User user, Guid secretKey);
    bool ValidateGoogleAuthCode(TwoStepVerificationGoogleAuthInputDto model);
    (bool, string) ValidateCaptcha(string response);
    int GetLastSentSmsRemainingTime(string mobile);
    int GetLastEmailCodeRemainingTime(string email);

    void LogFailureLogin(string username, UserSystemDataDto model);
    void LogSuccessLogin(Guid userId, string username, UserSystemDataDto model);
    void LogSuccessLogout(Guid userId, string username, UserSystemDataDto model);
}