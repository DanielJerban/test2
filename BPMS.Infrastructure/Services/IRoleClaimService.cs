using BPMS.Domain.Common.Dtos.Permission;
using BPMS.Domain.Entities;

namespace BPMS.Infrastructure.Services;

public interface IRoleClaimService
{
    List<RoleClaimDto> GetRoleClaims(Guid roleId);
    void CreateAndRemoveRoutePermission(List<string> claimValue, Guid roleId, string userName);
    void CreateAndRemoveWorkFlowPermission(List<Workflow> workFlows, Guid roleId, string userName);
    void CreateAndRemoveWorkFlowPreviewPermission(List<LookUp> lookUps, Guid roleId, string userName);
    void CreateAndRemoveWorkFlowFormPreviewPermission(List<WorkFlowForm> workFlowForms, Guid roleId, string userName);
    void CreateAndRemoveWorkFlowIndexPermission(List<LookUp> lookUps, Guid roleId, string userName);
    void CreateAndRemoveDynamicChartReportPermission(List<DynamicChart> charts, Guid roleId, string userName);
    void CreateAndRemoveWorkFlowFormListPermission(List<WorkFlowFormList> lists, Guid roleId, string userName);
    void CreateAndRemoveReportPermission(List<Report> reports, Guid roleId, string userName);
    void CreateAndRemoveWorkFlowStatusPermission(List<LookUp> lookUps, Guid roleId, string userName);
    void CreateAndRemoveWidgetPermission(List<Guid> modelWidgetIds, Guid roleId, string userName);
    void ResetcacheByRoleId(Guid roleId);
}