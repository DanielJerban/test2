using BPMS.Domain.Common.Dtos;
using BPMS.Domain.Common.Dtos.User;
using BPMS.Domain.Common.ViewModels;
using BPMS.Domain.Entities;
using System.Collections;
using BPMS.Infrastructure.Repositories.Base;

namespace BPMS.Infrastructure.Repositories;

public interface IUserRepository : IRepository<User>
{
    IEnumerable<StaffViewModel> GetStaffs();
    IEnumerable GetStaffAndUsers();
    StaffDto ExternalLogin(UserDto model);
    void Logout(UserSystemDataDto model);
    User FindUser(string username, string password);
    UserLoginOutSimpleDto GetUserLastSuccessLogin(string username);
}