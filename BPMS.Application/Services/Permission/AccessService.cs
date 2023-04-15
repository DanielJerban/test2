using BPMS.Domain.Common.Constants.PermissionStructure;
using BPMS.Domain.Common.ViewModels;
using BPMS.Infrastructure;
using BPMS.Infrastructure.MainHelpers;
using BPMS.Infrastructure.Services;
using Kendo.Mvc.UI;
using Microsoft.Extensions.Configuration;

namespace BPMS.Application.Services.Permission;

public class AccessService : IAccessService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IRoleClaimService _roleClaimService;
    private readonly IUserClaimService _userClaimService;
    private readonly IDistributedCacheHelper _cacheHelper;
    private readonly IStaffService _staffService;
    private readonly IConfiguration _configuration;

    public AccessService(IUnitOfWork unitOfWork, IRoleClaimService roleClaimService, IUserClaimService userClaimService, IDistributedCacheHelper cacheHelper, IStaffService staffService, IConfiguration configuration)
    {
        _unitOfWork = unitOfWork;
        _roleClaimService = roleClaimService;
        _userClaimService = userClaimService;
        _cacheHelper = cacheHelper;
        _staffService = staffService;
        _configuration = configuration;
    }

    public List<string> GetPermissionByRole(Guid? id)
    {
        List<string> roleAccess = new List<string>();

        if (id != null)
        {
            var roleId = (Guid)id;
            var roleClaims = _roleClaimService.GetRoleClaims(roleId);
            roleAccess = roleClaims?.Where(c => c.ClaimType == PermissionPolicyType.RoutePermission)
                .Select(i => i.ClaimValue).ToList();
        }

        return roleAccess?.Distinct().ToList();

    }

    public List<string> GetUserPermissions(Guid userId)
    {
        var user = _unitOfWork.Users.Single(c => c.Id == userId);

        var claimType = PermissionPolicyType.RoutePermission;
        List<string> allAccesses = new List<string>();

        var userClaims = _userClaimService.GetUserClaims(userId);
        var userRoleIds = _staffService.GetRoleIds(userId).Distinct().ToList();


        var roleMapPostTypeRoleIds = _unitOfWork.RoleAccesses.GetRoleMapPostTypeAccessId(user.StaffId);

        foreach (var roleId in roleMapPostTypeRoleIds)
        {
            var postTypeRoleIdRouteAccess = _roleClaimService.GetRoleClaims(roleId).Where(c => c.ClaimType == claimType);
            if (postTypeRoleIdRouteAccess.Any())
            {
                userRoleIds.Add(roleId);
            }
        }

        var roleMapPostTitleRoleIds = _unitOfWork.RoleAccesses.GetRoleMapPostTitleAccessId(user.StaffId);

        foreach (var roleId in roleMapPostTitleRoleIds)
        {
            var postTitleeRoleIdRouteAccess = _roleClaimService.GetRoleClaims(roleId).Where(c => c.ClaimType == claimType);
            if (postTitleeRoleIdRouteAccess.Any())
            {
                userRoleIds.Add(roleId);
            }
        }

        List<string> userAccess = userClaims?.Where(i => i.ClaimType == PermissionPolicyType.RoutePermission).Select(i => i.ClaimValue).ToList();
        if (userAccess != null)
        {
            allAccesses.AddRange(userAccess);
        }

        var chartAccessRoleIds = _unitOfWork.RoleAccesses.GetChartAccessIds(user.StaffId);

        foreach (var roleId in chartAccessRoleIds)
        {
            var chartAccessRoleIdRouteAccess = _roleClaimService.GetRoleClaims(roleId).Where(c => c.ClaimType == claimType);
            if (chartAccessRoleIdRouteAccess.Any())
            {
                userRoleIds.Add(roleId);
            }
        }

        foreach (var roleId in userRoleIds)
        {
            var roleClaims = _roleClaimService.GetRoleClaims(roleId);
            List<string> roleAccess = roleClaims?.Where(c => c.ClaimType == PermissionPolicyType.RoutePermission).Select(i => i.ClaimValue).ToList();
            allAccesses.AddRange(roleAccess);
        }

        GetOrSetUserAccesses(userId, allAccesses);

        return allAccesses.Distinct().ToList();
    }

    public List<AccessListItem> CreateMenueTreeView(Guid userId)
    {
        var userPermissions = GetUserPermissions(userId);
        var items = AccessList.GetAccessList();
        var accessitems = items.Where(i => i.ConsiderInMenue).ToList();
        var result = GroupEnumerable.BuildTree(accessitems, userPermissions, _configuration);
        return result.ToList();

    }
    public List<AccessListItem> CreateUsersAllAccessTreeView(Guid userId)
    {
        var userPermissions = GetUserPermissions(userId);
        var accessitems = AccessList.GetAccessList();
        return GroupEnumerable.BuildTree(accessitems, userPermissions, _configuration).ToList();

    }


    public List<TreeViewItemModel> GetTreeAccessPolicyBase(Guid? roleId)
    {
        var userPermissions = GetPermissionByRole(roleId);
        var treeViewList = new List<TreeViewItemModel>();
        var accessitems = AccessList.GetAccessList();
        var result = GroupEnumerable.BuildTree(accessitems, userPermissions, _configuration).ToList();
        treeViewList = createNewTreeViewItemModel(result, userPermissions).ToList();
        return treeViewList;
    }


    private IEnumerable<TreeViewItemModel> createNewTreeViewItemModel(List<AccessListItem> source, List<string> userPermissions)
    {
        var treeViewList = new List<TreeViewItemModel>();

        foreach (var sourceItem in source)
        {

            var controller = new TreeViewItemModel
            {
                Text = sourceItem.PersianName,
                Id = sourceItem.ClaimGuid
            };
            controller.Checked = chechIfHasAccess(sourceItem, userPermissions);
            foreach (var child in sourceItem.Children)
            {
                var action = new TreeViewItemModel
                {
                    Text = child.PersianName,
                    Id = child.ClaimGuid
                };


                foreach (var sub in child.Children)
                {
                    var newchild = new TreeViewItemModel
                    {
                        Text = sub.PersianName,
                        Id = sub.ClaimGuid
                    };

                    newchild.Checked = chechIfHasAccess(sub, userPermissions);
                    action.Items.Add(newchild);
                }

                controller.Items.Add(action);

                action.Checked = chechIfHasAccess(child, userPermissions);

            }

            treeViewList.Add(controller);

        }

        return treeViewList;

    }

    private bool chechIfHasAccess(AccessListItem child, List<String> userPermissions)
    {
        var result = false;
        var claimValue = (string.IsNullOrEmpty(child.ClaimGuid) ? "" : child.ClaimGuid);

        if (userPermissions.Contains(claimValue))
        {
            result = true;
        }

        return result;
    }

    private void GetOrSetUserAccesses(Guid userId, List<string> allAccesses)
    {
        var cachekey = CacheKeyHelper.GetUserAccessCacheKey(userId);
        _cacheHelper.Remove(cachekey);
        _cacheHelper.GetOrSet
            (cachekey, () => allAccesses, TimeSpan.FromDays(45));
    }
    // private string GetUserAccessCacheKey(Guid userId) => $"UserAccessCacheKey_{userId}";

}
public static class GroupEnumerable
{
    public static IList<AccessListItem> BuildTree(IEnumerable<AccessListItem> source, List<string> userPermission, IConfiguration configuration)
    {
        var groups = source.GroupBy(i => i.ParentId);
        var roots = groups.FirstOrDefault(g => g?.Key.HasValue == false)
            .ToList();

        if (roots.Count > 0)
        {
            var dict = groups.Where(g => g.Key.HasValue).ToDictionary(g => g.Key.Value, g => g.ToList());
            for (int i = 0; i < roots.Count; i++)
                AddChildren(roots[i], dict, userPermission, configuration);
        }

        return roots;
    }

    private static void AddChildren(AccessListItem node, IDictionary<int, List<AccessListItem>> source, List<string> userPermission, IConfiguration configuration)
    {

        if (source.ContainsKey(node.Id))
        {

            node.Children = source[node.Id];
            for (int i = 0; i < node.Children.Count; i++)
                AddChildren(node.Children[i], source, userPermission, configuration);
        }
        else
        {
            node.Children = new List<AccessListItem>();
            if (userPermission.Contains(node.ClaimGuid))
            {
                if (node.Title == "GetJiraLogs" && configuration["CommonIsJiraEnabled"] == "false")
                {
                    node.Checked = false;
                }
                else
                {
                    node.Checked = true;
                }

            }
        }
    }


}