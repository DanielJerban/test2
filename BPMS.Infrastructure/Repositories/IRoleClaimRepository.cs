using BPMS.Domain.Common.Dtos.Permission;
using BPMS.Domain.Entities;
using BPMS.Infrastructure.Repositories.Base;

namespace BPMS.Infrastructure.Repositories;

public interface IRoleClaimRepository : IRepository<RoleClaim>
{
    IQueryable<RoleClaimDto> GetRoleClaims(Guid roleId);
    bool RemoveRoleClaimsForType(Guid roleId, string claimType, List<string> claimValues);
    bool InsertRoleClaimsForType(Guid roleId, string claimType, List<string> claimValues);
    bool RemoveRange(IEnumerable<Guid> claimIds);
    IQueryable<RoleClaim> GetAllRolesClaims();
    RoleClaim GetById(Guid id);
}