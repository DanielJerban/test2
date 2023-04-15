using BPMS.Domain.Common.ViewModels;
using BPMS.Domain.Entities;
using Kendo.Mvc.UI;
using BPMS.Infrastructure.Repositories.Base;

namespace BPMS.Infrastructure.Repositories;

public interface IRoleRepository : IRepository<Role>
{
    IEnumerable<RoleViewModel> GetRolesToFillGrid();
    IEnumerable<UserViewModel> GetUsers();
    IEnumerable<UserViewModel> UsersInSpecificRole(Guid? id);
    IEnumerable<UserViewModel> UsersInRoles(List<Guid> id);
    IEnumerable<UserViewModel> UsersInRoles2(Guid[] id);
    IEnumerable<ReportAccessViewModel> GetAccessesForReports();
    IEnumerable<ChartViewModel> GetCharts();
    IEnumerable<ChartViewModel> ChartsInRoles(List<Guid> id);
    IEnumerable<ChartViewModel> ChartsInSpecificRole(Guid? id);
    IEnumerable<WorkFlowViewModel> GetDiagrams();
    IEnumerable<WorkFlowViewModel> DiagramsInRole(List<Guid> id);
    List<TreeViewItemModel> GetTreeWidget(Guid? id);
    void ModifyRoleMapChart(List<Chart> modelCharts, Guid roleId);
    void CreateNewRole(RoleViewModel model);
    void DeleteRole(Guid id);
    bool CheckUserIsAdmin(Guid userId);
    IEnumerable<WorkFlowViewModel> GetProcess();
    IEnumerable<TreeViewItemModel> GetTreeAccessPolicyBase(Guid? id);
    IEnumerable<WorkFlowViewModel> DiagramsInSpecificRoleByPolicy(Guid? id);
    List<WorkFlowViewModel> ProcessBpmnInSpecificRoleByPolicy(Guid? roleId);
    List<WorkFlowFormViewModel> FormsInSpecificRoleByPolicy(Guid? id);
    List<WorkFlowViewModel> WorkFlowIndexAccessByPolicy(Guid? roleId);
    List<DynamicChartViewModel> DynamicChartsInSpecificRoleByPolicy(Guid? id);
    List<WorkFlowFormListViewModel> WorkflowFormListInSpecificRoleByPolicy(Guid? id);
    List<ReportViewModel> GeneratedReportsInSpecificRolePolicy(Guid? id);
    List<WorkFlowViewModel> ProcessStatusInSpecificRolePolicy(Guid? id);
    List<WorkFlowViewModel> GetProcessesList();
    List<WorkFlowViewModel> GetProcessByIds(List<Guid> ids);
    IEnumerable<PostTypeViewModel> PostTypeInSpecificRole(Guid? id);
    void ModifyRoleMapPostType(List<LookUp> modelPostTypes, Guid roleId);
    void ModifyRoleMapPostTitle(List<LookUp> modelPostTitles, Guid roleId);
    IEnumerable<PostTitleViewModel> PostTitleInSpecificRole(Guid? id);
    List<Guid> GetUserRoles(string username);
}