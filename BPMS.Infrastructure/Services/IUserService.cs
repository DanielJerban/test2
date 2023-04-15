using BPMS.Domain.Common.Dtos;
using BPMS.Domain.Common.ViewModels;
using BPMS.Domain.Common.ViewModels.Global;
using BPMS.Domain.Entities;

namespace BPMS.Infrastructure.Services;

public interface IUserService
{
    UserBaseInfoCacheDTO GetUser(string userName);
    GetUserFullDataByUsernameDTO GetUserFullDataByUsername(string userName);
    void ResetUserFullDataFromCache(string username);
    Result UpdateTwoStepVerification(User user, TwoStepVerificationViewModel model);
    UserProfileInfoViewModel GetUserInfo(string currentUserName);
    Result SetGoogleAuthKey(string email, string googleAuthKey);
    void ModifyRoleMapUser(List<User> users, Guid roleId);
}