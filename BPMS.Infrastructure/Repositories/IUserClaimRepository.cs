using BPMS.Domain.Common.Dtos.Permission;
using BPMS.Domain.Entities;
using BPMS.Infrastructure.Repositories.Base;

namespace BPMS.Infrastructure.Repositories;

public interface IUserClaimRepository : IRepository<UserClaim>
{
    IQueryable<UserClaimDto> GetUserClaims(Guid userId);
    bool RemoveUserClaimsForType(Guid userId, string claimType, List<string> claimValues);
    bool InsertUserClaimsForType(Guid userId, string claimType, List<string> claimValues);
    bool RemoveRange(IEnumerable<Guid> claimIds);
    IQueryable<UserClaim> GetAllUsersClaims();
    UserClaim GetById(Guid id);

}