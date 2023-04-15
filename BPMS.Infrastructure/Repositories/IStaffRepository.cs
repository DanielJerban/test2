using BPMS.Domain.Common.Dtos;
using BPMS.Domain.Common.ViewModels;
using BPMS.Domain.Entities;
using BPMS.Infrastructure.Repositories.Base;
using Microsoft.AspNetCore.Http;

namespace BPMS.Infrastructure.Repositories;

public interface IStaffsRepository : IRepository<Staff>
{
    IEnumerable<StaffDto> GetAllActiveStaff();
    IEnumerable<StaffViewModel> GetAllStaffForstaffViewMOdel();
    void CheckStaffFields(string email, string personalCode, string phone);
    Staff? GetCurrentEditedStaff(string personalCode);
    Staff GetStaffByPersonelCode(string personelCode);
    IEnumerable<UserViewModel> GetStaffsHaveEmail();
    IEnumerable<UserViewModel> GetStaffsHavePhoneNumber();
    StaffViewModel GetStaffDetail(Guid id);
    IEnumerable<StaffDto> GetAllStaff();
    UserDefultDto GetUserDefaultStaff();
    IEnumerable<DynamicViewModel> GetAllFileUpload(Guid? id, string webRootPath);
    void FilePerPerson(Guid id, IFormFile file, string webRootPath);
    string Download(Guid id, string name, string webRootPath);
    List<Guid> GetRoleIds(Guid userId);
    IEnumerable<StaffDto> GetStaffsByParentId(bool isActive);
    List<HeadStaffDto> GetHeadersOfThisStaff(Guid staffId);
    List<StaffDto> GetAdminSubStaffs(Guid staffId);
    List<string> GetRoles(Guid userId);
    bool EmailExistance(string email, Guid staffId);
    Guid? GetStaffMainOrganizationInfoId(Guid staffId);
    string GetCompanyName(Guid? chartId);
    Staff GetStaffById(Guid staffId);
    IQueryable<Guid> GetStaffIdsByWorkFlowDetail(WorkFlowDetail nextWorkFlowDetail);
    IEnumerable<StaffRequestsDto> GetRequestCountByStaff(Guid staffId,int startDate, int endDate);
}