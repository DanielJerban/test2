using BPMS.Domain.Common.Constants.PermissionStructure;
using BPMS.Domain.Common.Dtos.Permission;
using BPMS.Domain.Common.Enums;
using BPMS.Domain.Entities;
using BPMS.Infrastructure;
using BPMS.Infrastructure.MainHelpers;
using BPMS.Infrastructure.Repositories;
using BPMS.Infrastructure.Services;

namespace BPMS.Application.Services.Permission;

public class RoleClaimService : IRoleClaimService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IPermissionLogsRepository _permissionLogsRepository;
    private readonly IDistributedCacheHelper _cacheHelper;

    public RoleClaimService(IUnitOfWork unitOfWork, IPermissionLogsRepository permissionLogsRepository, IDistributedCacheHelper cacheHelper)
    {
        _unitOfWork = unitOfWork;
        _cacheHelper = cacheHelper;
        _permissionLogsRepository = permissionLogsRepository;
    }

    public List<RoleClaimDto> GetRoleClaims(Guid roleId)
    {
        string cacheKey = CacheKeyHelper.GetRoleClaimsCacheKey(roleId);
        return _cacheHelper.GetOrSet(cacheKey, () => _unitOfWork.RoleClaim.GetRoleClaims(roleId).ToList(), TimeSpan.FromDays(45));
    }

    public void CreateAndRemoveRoutePermission(List<string> claimValues, Guid roleId, string userName)
    {
        string claimType = PermissionPolicyType.RoutePermission;
        CreateAndRemoveRoleClaim(claimValues, roleId, claimType, userName);
    }

    public void CreateAndRemoveWorkFlowPermission(List<Workflow> workFlows, Guid roleId, string userName)
    {
        string claimType = PermissionPolicyType.WorkFlowPermission;
        CreateAndRemoveRoleClaim(workFlows.Select(c => c.RequestTypeId.ToString()).ToList(), roleId, claimType, userName);
    }

    public void CreateAndRemoveWorkFlowPreviewPermission(List<LookUp> requestTypes, Guid roleId, string userName)
    {
        string claimType = PermissionPolicyType.WorkFlowPreviewPermission;
        CreateAndRemoveRoleClaim(requestTypes.Select(c => c.Id.ToString()).ToList(), roleId, claimType, userName);
    }

    public void CreateAndRemoveWorkFlowFormPreviewPermission(List<WorkFlowForm> workFlowForms, Guid roleId, string userName)
    {
        string claimType = PermissionPolicyType.WorkFlowFormPreviewPermission;
        CreateAndRemoveRoleClaim(workFlowForms.Select(c => c.Id.ToString()).ToList(), roleId, claimType, userName);
    }

    public void CreateAndRemoveWorkFlowIndexPermission(List<LookUp> lookUps, Guid roleId, string userName)
    {
        string claimType = PermissionPolicyType.WorkFlowIndexPermission;
        CreateAndRemoveRoleClaim(lookUps.Select(c => c.Id.ToString()).ToList(), roleId, claimType, userName);
    }

    public void CreateAndRemoveDynamicChartReportPermission(List<DynamicChart> charts, Guid roleId, string userName)
    {
        string claimType = PermissionPolicyType.DynamicChartReportPermission;
        CreateAndRemoveRoleClaim(charts.Select(c => c.Id.ToString()).ToList(), roleId, claimType, userName);
    }

    public void CreateAndRemoveWorkFlowFormListPermission(List<WorkFlowFormList> lists, Guid roleId, string userName)
    {
        string claimType = PermissionPolicyType.WorkFlowFormListPermission;
        CreateAndRemoveRoleClaim(lists.Select(c => c.Id.ToString()).ToList(), roleId, claimType, userName);
    }

    public void CreateAndRemoveReportPermission(List<Report> reports, Guid roleId, string userName)
    {
        string claimType = PermissionPolicyType.ReportPermission;
        CreateAndRemoveRoleClaim(reports.Select(c => c.Id.ToString()).ToList(), roleId, claimType, userName);
    }

    public void CreateAndRemoveWorkFlowStatusPermission(List<LookUp> lookUps, Guid roleId, string userName)
    {
        string claimType = PermissionPolicyType.WorkFlowStatusPermission;
        CreateAndRemoveRoleClaim(lookUps.Select(c => c.Id.ToString()).ToList(), roleId, claimType, userName);
    }

    public void CreateAndRemoveWidgetPermission(List<Guid> modelWidgetIds, Guid roleId, string userName)
    {
        string claimType = PermissionPolicyType.WidgetPermission;
        CreateAndRemoveRoleClaim(modelWidgetIds.Select(c => c.ToString()).ToList(), roleId, claimType, userName);
    }

    public void ResetcacheByRoleId(Guid roleId)
    {

        ResetCache(roleId);

    }

    private void CreateAndRemoveRoleClaim(List<string> claimValues, Guid roleId, string claimType, string userName)
    {
        var roleClaims = _unitOfWork.RoleClaim.Where(c => c.RoleId == roleId && c.ClaimType == claimType);

        var currentClaimValues = roleClaims.Select(c => c.ClaimValue);

        var addedClaimValues = claimValues.Except(currentClaimValues).ToList();

        var claimsToDelete = currentClaimValues.Except(claimValues).ToList();

        var roleClaimsToDelete = roleClaims.Where(a => claimsToDelete.Contains(a.ClaimValue));


        _unitOfWork.RoleClaim.RemoveRange(roleClaimsToDelete);

        foreach (var item in addedClaimValues)
        {
            _unitOfWork.RoleClaim.Add(new RoleClaim()
            {
                RoleId = roleId,
                ClaimType = claimType,
                ClaimValue = item
            });
        }

        _unitOfWork.Complete();

        Task.Factory.StartNew(() => AddPermissionLog(roleId, claimType, addedClaimValues, claimsToDelete, userName));

        ResetCache(roleId);

    }

    private void ResetCache(Guid roleId) => _cacheHelper.Remove((CacheKeyHelper.GetRoleClaimsCacheKey(roleId)));

    private void AddPermissionLog(Guid roleId, string claimType, IEnumerable<string> addedClaims, IEnumerable<string> removedClaims, string userName)
    {
        var roleName = _unitOfWork.Roles.Find(a => a.Id == roleId).First().Name;

        var now = DateTime.Now;

        var userId = _unitOfWork.Users.Find(a => a.UserName == userName).First().Id;

        var added = addedClaims.Select(c => new ClaimValueModel() { ClaimValue = c, ActionType = PermissionActionType.Add });
        var deleted = removedClaims.Select(d => new ClaimValueModel() { ClaimValue = d, ActionType = PermissionActionType.Delete });

        var permissionLogs = added.Union(deleted).Select(item => new PermissionLog()
        {
            ClaimType = claimType,
            ClaimValue = item.ClaimValue,
            RoleId = roleId,
            RoleName = roleName,
            CreatorUserId = userId,
            ActionType = item.ActionType,
            CreateDateTime = DateTime.Now,
            CreateDate = Convert.ToInt32(now.ToString("yyyyMMdd")),
            CreateTime = now.ToString("HHmm")
        }).ToList();

        switch (claimType)
        {
            case PermissionPolicyType.RoutePermission:
                SetRouteDescription(permissionLogs);
                break;
            case PermissionPolicyType.WidgetPermission:
                SetWidgetDescription(permissionLogs);
                break;
            case PermissionPolicyType.WorkFlowPermission:
                SetWorkFlowDescription(permissionLogs);
                break;
            case PermissionPolicyType.ReportPermission:
                SetReportDescription(permissionLogs);
                break;
            case PermissionPolicyType.WorkFlowFormListPermission:
                SetWorkFlowFormListDescription(permissionLogs);
                break;
            case PermissionPolicyType.WorkFlowFormPreviewPermission:
                SetWorkFlowFormPreviewDescription(permissionLogs);
                break;
            case PermissionPolicyType.DynamicChartReportPermission:
                SetDynamicChartReportDescription(permissionLogs);
                break;
            case PermissionPolicyType.WorkFlowPreviewPermission:
                SetWorkFlowDescription(permissionLogs);
                break;
            case PermissionPolicyType.WorkFlowIndexPermission:
                SetWorkFlowDescription(permissionLogs);
                break;
            case PermissionPolicyType.WorkFlowStatusPermission:
                SetWorkFlowDescription(permissionLogs);
                break;


        }

        _permissionLogsRepository.AddPermissionLog(permissionLogs);
    }

    private void SetRouteDescription(IEnumerable<PermissionLog> logs)
    {
        var accessList = AccessList.GetAccessList();
        foreach (var item in logs)
        {
            var root = accessList.FirstOrDefault(a => a.PersianName == item.ClaimValue);
            if (root != null)
            {
                item.ClaimDesc = root.PersianName;
                continue;
            }

            var route = accessList.FirstOrDefault(a => a.ClaimGuid == item.ClaimValue);

            if (route != null)
            {
                string routeName = route.PersianName;
                while (route.ParentId != null)
                {
                    var parent = accessList.First(a => a.Id == route.ParentId);
                    routeName = string.Concat($"{parent.PersianName} - ", routeName);
                    route = parent;
                }

                item.ClaimDesc = routeName;
            }
        }

    }

    private void SetWidgetDescription(IEnumerable<PermissionLog> logs)
    {
        var widgets = _unitOfWork.LookUps.Find(a => a.Type == "Widget");
        foreach (var log in logs)
        {
            log.ClaimDesc = widgets.FirstOrDefault(w => w.Id.ToString() == log.ClaimValue).Title;
        }
    }

    private void SetWorkFlowDescription(IEnumerable<PermissionLog> logs)
    {
        var widgets = _unitOfWork.LookUps.Find(a => a.Type == "RequestType");
        foreach (var log in logs)
        {
            log.ClaimDesc = widgets.FirstOrDefault(w => w.Id.ToString() == log.ClaimValue).Title;
        }
    }
    private void SetReportDescription(IEnumerable<PermissionLog> logs)
    {
        var widgets = _unitOfWork.Reports.GetAll();
        foreach (var log in logs)
        {
            log.ClaimDesc = widgets.FirstOrDefault(w => w.Id.ToString() == log.ClaimValue).Title;
        }
    }
    private void SetWorkFlowFormListDescription(IEnumerable<PermissionLog> logs)
    {
        var widgets = _unitOfWork.WorkFlowFormLists.GetAll();
        foreach (var log in logs)
        {
            log.ClaimDesc = widgets.FirstOrDefault(w => w.Id.ToString() == log.ClaimValue).Title;
        }
    }
    private void SetWorkFlowFormPreviewDescription(IEnumerable<PermissionLog> logs)
    {
        var widgets = _unitOfWork.WorkFlowForm.GetAll();
        foreach (var log in logs)
        {
            log.ClaimDesc = widgets.FirstOrDefault(w => w.Id.ToString() == log.ClaimValue).PName;
        }
    }
    private void SetDynamicChartReportDescription(IEnumerable<PermissionLog> logs)
    {
        var widgets = _unitOfWork.DynamicCharts.GetAll();
        foreach (var log in logs)
        {
            log.ClaimDesc = widgets.FirstOrDefault(w => w.Id.ToString() == log.ClaimValue).WidgetType.Title;
        }
    }
}