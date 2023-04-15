using BPMS.Domain.Common.ViewModels;
using Kendo.Mvc.UI;

namespace BPMS.Infrastructure.Services;

public interface IAccessService
{
    List<string> GetPermissionByRole(Guid? id);
    List<TreeViewItemModel> GetTreeAccessPolicyBase(Guid? roleId);
    List<AccessListItem> CreateMenueTreeView(Guid userId);
    List<AccessListItem> CreateUsersAllAccessTreeView(Guid userId);
    List<string> GetUserPermissions(Guid userId);
}