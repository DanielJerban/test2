using BPMS.Domain.Common.Dtos;
using BPMS.Domain.Common.ViewModels;
using BPMS.Domain.Common.ViewModels.Global;
using BPMS.Domain.Entities;
using System.Collections;
using BPMS.Infrastructure.Repositories.Base;

namespace BPMS.Infrastructure.Repositories;

public interface IOrganizationInfoRepository : IRepository<OrganiztionInfo>
{
    IEnumerable<ChartTreeViewModel> GetChartTreeViewModel(Guid id);
    void HaveAnyMainPost(Guid staffId, bool isMain);
    void OldMainPostDisinfect(Guid staffId);
    IEnumerable<StaffPostViewModel> GetPostsToFillGrid(Guid staffId);
    IEnumerable<StaffViewModel> GetAllStaff();
    IEnumerable<StaffViewModel> GetStaffbyIds(List<Guid> ids);
    IEnumerable FillTree(Guid id);
    List<LookUp> GetLookUpsByType();
    Guid ExistsInWorkFlowDetail(Guid staffId);
    Result CheckRedundantPostForSamePerson(Guid chartId, Guid staffId, Guid organizationPostId, Guid postId);
    void CheckForPostsCapacity(Guid chartId, Guid organizationPostId, Guid staffId);
    void CheckForActiveFlowForCurrentStaff(Guid staffId, Guid organizationPostId);
    void CheckForAtLeastOnePost(Guid staffId, bool mainPost, bool isActive);
    void CheckForActiveMainPostInEdit(Guid staffId, bool mainPost, bool active, Guid postId);
    IEnumerable<OrganizationInfoDto> GetAllOrganizationInfo();
    void UpdateModifeidOrganizationInfos(OrganizationInfoPhpDto model);
    OrganiztionInfo GetOrgInfoByStaffId(Guid staffId);

    List<OrganiztionInfo> GetOrganizationInfosTypeEngTypeBystaffIdAndChartId(Guid chartId, WorkflowDetailPatternItem patternItem,
        Guid staffId);

    List<OrganiztionInfo> GetOrganizationInfosBystaffIdAndChartId(Guid chartId, WorkFlowDetail nextWorkFlowDetail,
        Guid staffId);

    List<OrganiztionInfo> GetOrgInfos();
    OrganiztionInfo GetOrgByStaffIdAndOrgPostId(Guid staffId, Guid organizationPostId);
}