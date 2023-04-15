using BPMS.Domain.Common.Constants.PermissionStructure;
using BPMS.Domain.Common.Enums;
using BPMS.Domain.Common.ViewModels;
using BPMS.Domain.Entities;
using BPMS.Infrastructure;
using BPMS.Infrastructure.Services;
using Kendo.Mvc.UI;

namespace BPMS.Application.Services.Permission;

public class PermissionService : IPermissionService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IRoleClaimService _roleClaimService;
    private readonly IUserClaimService _userClaimService;
    private readonly IAccessService _accessService;
    private readonly IStaffService _staffService;
    private readonly IUserService _userService;

    public PermissionService(IUnitOfWork unitOfWork, IRoleClaimService roleClaimService,
        IUserClaimService userClaimService, IAccessService accessService,
        IStaffService staffService, IUserService userService)
    {
        _unitOfWork = unitOfWork;
        _roleClaimService = roleClaimService;
        _userClaimService = userClaimService;
        _accessService = accessService;
        _staffService = staffService;
        _userService = userService;
    }

    public void CreateAccess(AccessViewModel model, PermissionGrantedType grantedType, AccessType accessType, string username)
    {
        switch (grantedType)
        {
            case PermissionGrantedType.User:
                var users = model.Users.ToList();
                _userService.ModifyRoleMapUser(users, model.Role.Id);
                break;
            case PermissionGrantedType.Chart:
                var charts = model.Charts.ToList();
                _unitOfWork.Roles.ModifyRoleMapChart(charts, model.Role.Id);
                break;
            case PermissionGrantedType.PostType:
                var postTypes = model.PostTypes.ToList();
                _unitOfWork.Roles.ModifyRoleMapPostType(postTypes, model.Role.Id);
                break;
            case PermissionGrantedType.PostTitle:
                var postTitles = model.PostTitles.ToList();
                _unitOfWork.Roles.ModifyRoleMapPostTitle(postTitles, model.Role.Id);
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(grantedType), grantedType, null);
        }

        switch (accessType)
        {
            case AccessType.Page:
                _roleClaimService.CreateAndRemoveRoutePermission(model.ActionRoles.ToList(), model.Role.Id,username);
                break;
            case AccessType.Widget:
                _roleClaimService.CreateAndRemoveWidgetPermission(model.WidgetIds.ToList(), model.Role.Id, username);
                break;
            case AccessType.Workflow:
                _roleClaimService.CreateAndRemoveWorkFlowPermission(model.Workflows.ToList(), model.Role.Id, username);
                break;
            case AccessType.WorkflowChangeSet:
                _roleClaimService.CreateAndRemoveWorkFlowPreviewPermission(model.ProcessBpmn.ToList(), model.Role.Id, username);
                break;
            case AccessType.FormChangeSet:
                _roleClaimService.CreateAndRemoveWorkFlowFormPreviewPermission(model.WorkFlowForms.ToList(), model.Role.Id, username);
                break;
            case AccessType.WorkflowIndex:
                _roleClaimService.CreateAndRemoveWorkFlowIndexPermission(model.ProcessIndicator.ToList(), model.Role.Id, username);
                break;
            case AccessType.DynamicChartChangeSet:
                _roleClaimService.CreateAndRemoveDynamicChartReportPermission(model.DynamicCharts.ToList(), model.Role.Id, username);
                break;
            case AccessType.Lists:
                _roleClaimService.CreateAndRemoveWorkFlowFormListPermission(model.WorkflowFormLists.ToList(), model.Role.Id, username);
                break;
            case AccessType.Reports:
                _roleClaimService.CreateAndRemoveReportPermission(model.ReportsGenerated.ToList(), model.Role.Id, username);
                break;
            case AccessType.WorkflowStatus:
                _roleClaimService.CreateAndRemoveWorkFlowStatusPermission(model.ProcessStatus.ToList(), model.Role.Id, username);
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(accessType), accessType, null);
        }

        _unitOfWork.Complete();
    }

    public OptimisedAccessViewModel GetAccessData(Guid? roleId, PermissionGrantedType? grantedType, AccessType? accessType)
    {
        List<UserViewModel> users = new List<UserViewModel>();
        List<ChartViewModel> charts = new List<ChartViewModel>();
        var postTypes = new List<PostTypeViewModel>();
        var postTitles = new List<PostTitleViewModel>();

        if (grantedType != null)
        {
            switch (grantedType)
            {
                case PermissionGrantedType.User:
                    users = _unitOfWork.Roles.UsersInSpecificRole(roleId).ToList();
                    break;
                case PermissionGrantedType.Chart:
                    charts = _unitOfWork.Roles.ChartsInSpecificRole(roleId).ToList();
                    break;
                case PermissionGrantedType.PostType:
                    postTypes = _unitOfWork.Roles.PostTypeInSpecificRole(roleId).ToList();
                    break;
                case PermissionGrantedType.PostTitle:
                    postTitles = _unitOfWork.Roles.PostTitleInSpecificRole(roleId).ToList();
                    break;
            }
        }

        var accessVm = new AccessViewModel()
        {
            Users = new List<User>(),
            Role = new Role(),
            ActionRoles = new List<string>(),
            Controllers = new List<TreeViewItemModel>(),
            Widgets = new List<TreeViewItemModel>()
        };

        var obj = new OptimisedAccessViewModel()
        {
            Users = users,
            Charts = charts,
            PostTypes = postTypes,
            PostTitles = postTitles
        };

        if (accessType != null)
        {
            switch (accessType)
            {
                case AccessType.Page: // RoutePermission
                    accessVm.Controllers = _accessService.GetTreeAccessPolicyBase(roleId);
                    break;
                case AccessType.Widget: // WidgetPermission
                    accessVm.Widgets = _unitOfWork.Roles.GetTreeWidget(roleId);
                    break;
                case AccessType.Workflow: // WorkFlowPermission 
                    obj.Data = _unitOfWork.Roles.DiagramsInSpecificRoleByPolicy(roleId);
                    break;
                case AccessType.WorkflowChangeSet: // WorkFlowPreviewPermission
                    obj.Data = _unitOfWork.Roles.ProcessBpmnInSpecificRoleByPolicy(roleId);
                    break;
                case AccessType.FormChangeSet: // WorkFlowFormPreviewPermission
                    obj.Data = _unitOfWork.Roles.FormsInSpecificRoleByPolicy(roleId);
                    break;
                case AccessType.WorkflowIndex: // WorkFlowIndexPermission
                    obj.Data = _unitOfWork.Roles.WorkFlowIndexAccessByPolicy(roleId);
                    break;
                case AccessType.DynamicChartChangeSet: // DynamicChartReportPermission
                    obj.Data = _unitOfWork.Roles.DynamicChartsInSpecificRoleByPolicy(roleId);
                    break;
                case AccessType.Lists: // WorkFlowFormListPermission
                    obj.Data = _unitOfWork.Roles.WorkflowFormListInSpecificRoleByPolicy(roleId);
                    break;
                case AccessType.Reports: // ReportPermission
                    obj.Data = _unitOfWork.Roles.GeneratedReportsInSpecificRolePolicy(roleId);
                    break;
                case AccessType.WorkflowStatus: // WorkFlowStatusPermission
                    obj.Data = _unitOfWork.Roles.ProcessStatusInSpecificRolePolicy(roleId);
                    break;
            }
        }

        obj.AccessViewModel = accessVm;

        return obj;
    }

    public bool CheckUserHasRoutePermission(Guid userId, string permissionClaimValue)
    {
        var claimType = PermissionPolicyType.RoutePermission;
        var userClaims = _userClaimService.GetUserClaims(userId);
        var hasAccess = userClaims.Any(i => i.ClaimValue == permissionClaimValue && i.ClaimType == PermissionPolicyType.RoutePermission);
        var user = _unitOfWork.Users.Single(c => c.Id == userId);

        if (hasAccess) return true;

        var userRoleIds = _staffService.GetRoleIds(userId);

        foreach (var roleId in userRoleIds)
        {
            var roleClaims = _roleClaimService.GetRoleClaims(roleId);
            hasAccess = roleClaims.Any(c => c.ClaimValue == permissionClaimValue && c.ClaimType == claimType);

            if (hasAccess)
            {
                return true;
            }
        }

        var chartAccessRoleIds = _unitOfWork.RoleAccesses.GetChartAccessIds(user.StaffId);

        foreach (var roleId in chartAccessRoleIds)
        {
            var chartAccessRoleIdRouteAccess = _roleClaimService.GetRoleClaims(roleId).Where(c => c.ClaimType == claimType);
            if (chartAccessRoleIdRouteAccess.Any())
            {
                return true;
            }
        }


        var roleMapPostTypeRoleIds = _unitOfWork.RoleAccesses.GetRoleMapPostTypeAccessId(user.StaffId);

        foreach (var roleId in roleMapPostTypeRoleIds)
        {
            var postTypeRoleIdRouteAccess = _roleClaimService.GetRoleClaims(roleId).Where(c => c.ClaimType == claimType);
            if (postTypeRoleIdRouteAccess.Any())
            {
                return true;
            }
        }
        var roleMapPostTitleRoleIds = _unitOfWork.RoleAccesses.GetRoleMapPostTitleAccessId(user.StaffId);

        foreach (var roleId in roleMapPostTitleRoleIds)
        {
            var postTitleRoleIdRouteAccess = _roleClaimService.GetRoleClaims(roleId).Where(c => c.ClaimType == claimType);
            if (postTitleRoleIdRouteAccess.Any())
            {
                return true;
            }
        }

        return false;
    }
}