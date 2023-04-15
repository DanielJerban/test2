using BPMS.Domain.Common.ViewModels;
using BPMS.Domain.Entities;
using BPMS.Infrastructure.Repositories.Base;

namespace BPMS.Infrastructure.Repositories;

public interface IUserSettingRepository : IRepository<UserSetting>
{
    void CreateUserSetting(WidgetViewModel[] data, string username);
    string FetchUserSettings(string username);
    string FetchAllUsersSettings();
}