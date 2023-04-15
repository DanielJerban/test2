using BPMS.Application.Repositories.Base;
using BPMS.Domain.Common.Dtos.Permission;
using BPMS.Domain.Entities;
using BPMS.Infrastructure.Data;
using BPMS.Infrastructure.Repositories;

namespace BPMS.Application.Repositories;

public class UserClaimRepository : Repository<UserClaim>, IUserClaimRepository
{
    public BpmsDbContext DbContext => Context;
    public UserClaimRepository(BpmsDbContext context) : base(context)
    {
    }


    public IQueryable<UserClaimDto> GetUserClaims(Guid userId)
    {
        var result = DbContext.UserClaims.Where(i => i.UserId == userId);
        return selectAsUserClaimDto(result);
    }

    public bool RemoveUserClaimsForType(Guid userId, string claimType, List<string> claimValues)
    {
        bool answer = false;
        try
        {

            foreach (var claimValue in claimValues)
            {
                var userclaim = DbContext.UserClaims.FirstOrDefault(d => d.UserId == userId && d.ClaimType == claimType && d.ClaimValue == claimValue);
                if (userclaim != null)
                {
                    answer = false;
                    DbContext.UserClaims.Remove(userclaim);
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


    public bool InsertUserClaimsForType(Guid userId, string claimType, List<string> claimValues)
    {
        bool answer = false;
        try
        {

            foreach (var claimValue in claimValues)
            {
                answer = false;
                var newUserClaim = new UserClaim()
                {
                    UserId = userId,
                    ClaimType = claimType,
                    ClaimValue = claimValue

                };
                DbContext.UserClaims.Add(newUserClaim);
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
        var userclaims = DbContext.UserClaims.Where(d => claimIds.Contains(d.Id));
        if (userclaims != null)
        {
            DbContext.UserClaims.RemoveRange(userclaims);
            DbContext.SaveChanges();
            answer = true;

        }
        return answer;
    }

    public IQueryable<UserClaim> GetAllUsersClaims()
    {
        return DbContext.UserClaims;
    }
    public UserClaim GetById(Guid id)
    {
        var userClaim = DbContext.UserClaims.FirstOrDefault(d => d.Id == id);
        return userClaim;
    }

    private IQueryable<UserClaimDto> selectAsUserClaimDto(IQueryable<UserClaim> userclaims)
    {
        return userclaims.Select(s => new UserClaimDto
        {
            Id = s.Id,
            UserId = s.UserId,
            ClaimValue = s.ClaimValue,
            ClaimType = s.ClaimType
        });
    }
}