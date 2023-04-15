using BPMS.Domain.Common.Dtos;
using BPMS.Domain.Common.ViewModels;

namespace BPMS.Infrastructure.Services;

public interface IUserLoginOutService
{
    void AddUserLoginOutLog(UserLoginOutDto userLoginOut);
    void DeleteUserLoginOutLog(Guid userId);
    IEnumerable<UserLoginOutViewModel> GetUserForLoginOut();
    IEnumerable<UserLoginOutViewModel> GetUserForLoginOut(string username);

}