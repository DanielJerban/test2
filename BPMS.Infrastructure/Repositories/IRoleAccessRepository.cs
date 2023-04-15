using BPMS.Domain.Entities;
using BPMS.Infrastructure.Repositories.Base;

namespace BPMS.Infrastructure.Repositories;

public interface IRoleAccessRepository : IRepository<RoleAccess>
{
    IEnumerable<RoleAccess> GetAllRoleAccesses();
    IQueryable<Guid> GetRoleMapPostTypeAccessId(Guid staffId);
    IQueryable<Guid> GetRoleMapPostTitleAccessId(Guid staffId);
    IQueryable<Guid> GetChartAccessIds(Guid staffId);
    void AddUserToRoleAccess(RoleAccess roleAccess);
    void RemoveUserFromRoleAccess(List<RoleAccess> roleAccesses);
}