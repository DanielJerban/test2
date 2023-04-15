using BPMS.Domain.Common.Dtos;
using BPMS.Domain.Common.ViewModels;

namespace BPMS.Infrastructure.Repositories;

public interface IUserLoginOutRepository
{
    void AddUserLoginOut(UserLoginOutDto userLoginOut);
    void DeleteUserLoginOutLog(Guid userId);
    IEnumerable<UserLoginOutViewModel> GetRecordsForLoginOut(string username);
}