using BPMS.Application.Repositories.Base;
using BPMS.Domain.Common.Dtos.Permission;
using BPMS.Domain.Entities;
using BPMS.Infrastructure.Data;
using BPMS.Infrastructure.Repositories;

namespace BPMS.Application.Repositories;

public class RoleClaimRepository : Repository<RoleClaim>, IRoleClaimRepository
{
    public BpmsDbContext DbContext => Context;
    public RoleClaimRepository(BpmsDbContext context) : base(context)
    {
    }


    public IQueryable<RoleClaimDto> GetRoleClaims(Guid roleId)
    {
        var result = DbContext.RoleClaims.Where(i => i.RoleId == roleId);
        return selectAsRoleClaimDto(result);
    }

    public bool RemoveRoleClaimsForType(Guid roleId, string claimType, List<string> claimValues)
    {
        bool answer = false;
        try
        {

            foreach (var claimValue in claimValues)
            {
                var roleclaim = DbContext.RoleClaims.FirstOrDefault(d => d.RoleId == roleId && d.ClaimType == claimType && d.ClaimValue == claimValue);
                if (roleclaim != null)
                {
                    answer = false;
                    DbContext.RoleClaims.Remove(roleclaim);
                    DbContext.SaveChanges();
                    answer = true;

                }
            }
        }
        catch (Exception ex)
        {

        }

        return answer;
    }


    public bool InsertRoleClaimsForType(Guid roleId, string claimType, List<string> claimValues)
    {
        bool answer = false;
        try
        {

            foreach (var claimValue in claimValues)
            {
                answer = false;
                var newRoleClaim = new RoleClaim()
                {
                    RoleId = roleId,
                    ClaimType = claimType,
                    ClaimValue = claimValue

                };
                DbContext.RoleClaims.Add(newRoleClaim);
                DbContext.SaveChanges();
                answer = true;

            }

        }
        catch (Exception ex)
        {

        }

        return answer;
    }

    public bool RemoveRange(IEnumerable<Guid> claimIds)
    {
        bool answer = false;
        var roleclaims = DbContext.RoleClaims.Where(d => claimIds.Contains(d.Id));
        if (roleclaims != null)
        {
            DbContext.RoleClaims.RemoveRange(roleclaims);
            DbContext.SaveChanges();
            answer = true;

        }
        return answer;
    }

    public IQueryable<RoleClaim> GetAllRolesClaims()
    {
        return DbContext.RoleClaims;
    }
    public RoleClaim GetById(Guid id)
    {
        var roleClaim = DbContext.RoleClaims.FirstOrDefault(d => d.Id == id);
        return roleClaim;
    }
    private IQueryable<RoleClaimDto> selectAsRoleClaimDto(IQueryable<RoleClaim> roleclaims)
    {
        return roleclaims.Select(s => new RoleClaimDto
        {
            Id = s.Id,
            RoleId = s.RoleId,
            ClaimValue = s.ClaimValue,
            ClaimType = s.ClaimType
        });
    }


}