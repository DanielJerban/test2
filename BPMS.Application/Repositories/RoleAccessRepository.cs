using BPMS.Application.Repositories.Base;
using BPMS.Domain.Entities;
using BPMS.Infrastructure.Data;
using BPMS.Infrastructure.Repositories;

namespace BPMS.Application.Repositories;

public class RoleAccessRepository : Repository<RoleAccess>, IRoleAccessRepository
{
    public RoleAccessRepository(BpmsDbContext context) : base(context)
    {
    }

    public BpmsDbContext DbContext => Context;


    public IEnumerable<RoleAccess> GetAllRoleAccesses()
    {
        return DbContext.RoleAccesses.ToList();
    }

    public IQueryable<Guid> GetRoleMapPostTitleAccessId(Guid staffId)
    {

        var userPostTitleIds = new List<Guid>();

        var userPostTitles = from orgInfo in DbContext.OrganiztionInfos
                             join orgPostTitle in DbContext.LookUps on orgInfo.OrganiztionPostId equals orgPostTitle.Id
                             where orgInfo.StaffId == staffId && orgInfo.IsActive

                             select new { orgInfo, orgPostTitle };

        foreach (var item in userPostTitles)
        {
            var postTitleId = DbContext.LookUps.Where(l => l.Id == item.orgInfo.OrganiztionPostId)
                .Select(o => o.Id).FirstOrDefault();
            userPostTitleIds.Add(postTitleId);

        }

        var roleMapPostTitleAccessId = from lookup in DbContext.LookUps
                                       join roleMapPostTitle in DbContext.RoleMapPostTitles
                                           on lookup.Id equals roleMapPostTitle.PostTitleId
                                       where userPostTitleIds.Contains(roleMapPostTitle.PostTitleId)
                                       select roleMapPostTitle.RoleId;

        return roleMapPostTitleAccessId;

    }
    public IQueryable<Guid> GetRoleMapPostTypeAccessId(Guid staffId)
    {
        var userPostTypes = from orgInfo in DbContext.OrganiztionInfos
                            join orgPostType in DbContext.LookUps on orgInfo.OrganiztionPostId equals orgPostType.Id
                            where orgInfo.StaffId == staffId && orgInfo.IsActive

                            select new { orgInfo, orgPostType };
        var userPostTypeIds = new List<Guid>();
        foreach (var item in userPostTypes)
        {
            var postTypeId = DbContext.LookUps.Where(l => l.Id == item.orgInfo.OrganiztionPostId)
                .Select(o => o.Aux).FirstOrDefault();
            if (postTypeId != null)
            {
                userPostTypeIds.Add(Guid.Parse(postTypeId));
            }
        }


        var roleMapPostTypeAccessId = from lookup in DbContext.LookUps
                                      join roleMapPostType in DbContext.RoleMapPostTypes
                                          on lookup.Id equals roleMapPostType.PostTypeId
                                      where userPostTypeIds.Contains(roleMapPostType.PostTypeId)
                                      select roleMapPostType.RoleId;

        return roleMapPostTypeAccessId;

    }
    public IQueryable<Guid> GetChartAccessIds(Guid staffId)
    {
        var chartAccessIds = from organizationInfo in DbContext.OrganiztionInfos
                             join chart in DbContext.Charts on organizationInfo.ChartId equals chart.Id
                             join roleMapChart in DbContext.RoleMapCharts on chart.Id equals roleMapChart.ChartId
                             where organizationInfo.StaffId == staffId
                             select roleMapChart.RoleId;

        return chartAccessIds;
    }


    public void AddUserToRoleAccess(RoleAccess roleAccess)
    {
        DbContext.RoleAccesses.Add(roleAccess);
        DbContext.SaveChanges();
    }

    public void RemoveUserFromRoleAccess(List<RoleAccess> roleAccesses)
    {
        DbContext.RoleAccesses.RemoveRange(roleAccesses);
        DbContext.SaveChanges();
    }
}