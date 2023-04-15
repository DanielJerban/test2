using BPMS.Domain.Common.Dtos;
using BPMS.Domain.Common.ViewModels;
using BPMS.Infrastructure.Repositories;
using BPMS.Infrastructure.Services;

namespace BPMS.Application.Services;

public class UserLoginOutService : IUserLoginOutService
{
    private readonly IUserLoginOutRepository _userLoginOutRepository;
    public UserLoginOutService(IUserLoginOutRepository userLoginOutRepository)
    {
        _userLoginOutRepository = userLoginOutRepository;
    }

    public void AddUserLoginOutLog(UserLoginOutDto userLoginOut)
    {
        _userLoginOutRepository.AddUserLoginOut(userLoginOut);
    }

    public void DeleteUserLoginOutLog(Guid userId)
    {
        _userLoginOutRepository.DeleteUserLoginOutLog(userId);
    }

    public IEnumerable<UserLoginOutViewModel> GetUserForLoginOut()
    {
        return _userLoginOutRepository.GetRecordsForLoginOut(null);
    }

    public IEnumerable<UserLoginOutViewModel> GetUserForLoginOut(string username)
    {
        return _userLoginOutRepository.GetRecordsForLoginOut(username);
    }
}