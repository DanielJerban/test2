using BPMS.Domain.Common.Dtos;
using BPMS.Domain.Common.Dtos.Staff;
using BPMS.Domain.Common.ViewModels.Global;

namespace BPMS.Infrastructure.Services;

public interface IStaffService
{
    string GetCompanyName(Guid? chartId);
    List<Guid> GetRoleIds(Guid userId);
    void ResetUserRoleCache(Guid userId);
    UserDefultDto GetDefultUser();
    Result EditStaff(EditStaffDto dto);
    void CreateStaff(CreateStaffDto dto);
    void CreateStaffFromActiveDirectory(ActiveDirectoryUserDto dto);
    void AddOrUpdateStaffFromAutomation(StaffPhpDto dto,string webRootPath);
    IEnumerable<StaffRequestsDto> GetRequestCountByStaff(string userName, string startDate,string endDate);
}