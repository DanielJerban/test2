using BPMS.Domain.Common.Enums;
using BPMS.Domain.Common.ViewModels;

namespace BPMS.Infrastructure.Services;

public interface IPermissionService
{
    void CreateAccess(AccessViewModel model, PermissionGrantedType grantedType, AccessType accessType, string username);
    OptimisedAccessViewModel GetAccessData(Guid? roleId, PermissionGrantedType? grantedType, AccessType? accessType);
    bool CheckUserHasRoutePermission(Guid userId, string permissionClaimValue);
}