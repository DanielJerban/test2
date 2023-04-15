using BPMS.Domain.Common.Dtos.Permission;

namespace BPMS.Infrastructure.Services;

public interface IUserClaimService
{
    List<UserClaimDto> GetUserClaims(Guid userId);
    bool SetPermissionsForUsers(List<ManageUserPermissionInputDTO> inputDtOs);
    bool RemovePermissionsForUser(Guid userId, IEnumerable<Guid> permissionIds);
    void CreateUserClaim(Guid userId, string claimType, string claimValue);
}