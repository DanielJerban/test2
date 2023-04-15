using BPMS.Domain.Common.Dtos.Permission;
using BPMS.Domain.Common.Enums;
using BPMS.Domain.Entities;
using BPMS.Infrastructure;
using BPMS.Infrastructure.MainHelpers;
using BPMS.Infrastructure.Services;

namespace BPMS.Application.Services.Permission;

public class UserClaimService : IUserClaimService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IDistributedCacheHelper _cacheHelper;

    public UserClaimService(IUnitOfWork unitOfWork, IDistributedCacheHelper cacheHelper)
    {
        _unitOfWork = unitOfWork;
        _cacheHelper = cacheHelper;
    }

    public List<UserClaimDto> GetUserClaims(Guid userId)
    {
        string cacheKey = CacheKeyHelper.GetUserClaimsCacheKey(userId);
        return _cacheHelper.GetOrSet(cacheKey, () => _unitOfWork.UserClaim.GetUserClaims(userId).ToList(), TimeSpan.FromDays(45));
    }

    public bool SetPermissionsForUsers(List<ManageUserPermissionInputDTO> inputDto)
    {
        List<Guid> userIds = inputDto.Select(a => a.UserId).Distinct().ToList();

        foreach (var userId in userIds)
        {
            List<string> permissionTypes = inputDto.Where(a => a.UserId == userId).Select(a => a.PermissionType).Distinct().ToList();

            foreach (var permissionType in permissionTypes)
            {
                var inputItemsByUser = inputDto.Where(p => p.UserId == userId && p.PermissionType == permissionType).ToList();
                var userItemsForDelete = inputItemsByUser.Where(p => p.PermissionManagementState == PermissionManagementState.Deleted).Select(p => p.PermissionValue).Distinct().ToList();
                var userItemsForAdd = inputItemsByUser.Where(p => p.PermissionManagementState == PermissionManagementState.Added).Select(p => p.PermissionValue).Distinct().ToList();

                if (userItemsForDelete.Count > 0)
                {
                    _unitOfWork.UserClaim.RemoveUserClaimsForType(userId, permissionType, userItemsForDelete);
                }

                if (userItemsForAdd.Count > 0)
                {
                    _unitOfWork.UserClaim.InsertUserClaimsForType(userId, permissionType, userItemsForAdd);
                }
            }

            ResetCache(userId);
        }

        return true;
    }

    public bool RemovePermissionsForUser(Guid userId, IEnumerable<Guid> permissionIds)
    {
        var claimIds = permissionIds.ToList();
        if (!claimIds.Any()) return true;
        bool success = _unitOfWork.UserClaim.RemoveRange(claimIds);

        if (success)
            ResetCache(userId);

        return true;
    }
    private void ResetCache(Guid userId) => _cacheHelper.Remove((CacheKeyHelper.GetUserClaimsCacheKey(userId)));
    public void CreateUserClaim(Guid userId, string claimType, string claimValue)
    {
        UserClaim userClaim = new UserClaim()
        {
            UserId = userId,
            ClaimType = claimType,
            ClaimValue = claimValue

        };

        _unitOfWork.UserClaim.Add(userClaim);
        _unitOfWork.Complete();
    }
}