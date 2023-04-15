using BPMS.Application.Repositories.Base;
using BPMS.Domain.Common.Constants.PermissionStructure;
using BPMS.Domain.Common.Dtos;
using BPMS.Domain.Common.ViewModels;
using BPMS.Domain.Entities;
using BPMS.Infrastructure.Data;
using BPMS.Infrastructure.Repositories;

namespace BPMS.Application.Repositories;

public class DynamicChartRepository : Repository<DynamicChart>, IDynamicChartRepository
{
    public BpmsDbContext DbContext => Context;


    public DynamicChartRepository(BpmsDbContext context) : base(context)
    {
    }

    public void SaveChart(DynamicChartViewModel model, string username)
    {
        if (model != null)
        {
            var user = DbContext.Users.Single(c => c.UserName == username);
            var chart = DbContext.DynamicCharts.Find(model.Id);
            if (chart == null)
            {
                int maxcode;
                var code = DbContext.LookUps.Where(l => l.Type == "Widget").Select(c => c.Code);
                if (code.Count() != 0)
                {
                    maxcode = code.Max() + 1;
                }
                else
                {
                    maxcode = 1;
                }
                var look = new LookUp()
                {
                    Aux = model.WidgetGroupTypeId.ToString(),
                    Code = maxcode,
                    Title = model.WidgetType?.Trim() ?? "",
                    IsActive = true,
                    Type = "Widget",
                    Aux2 = "Report"
                };
                DbContext.LookUps.Add(look);
                var diagram = new DynamicChart()
                {
                    Id = model.Id,
                    IsActive = model.IsActive,
                    DataSetting = model.DataSetting,
                    ReportId = model.ReportId,
                    WidgetTypeId = look.Id,
                    CreatorId = user.StaffId
                };
                DbContext.DynamicCharts.Update(diagram);
            }
            else
            {
                var widget = DbContext.LookUps.Find(chart.WidgetTypeId);
                widget.Title = model.WidgetType.Trim();
                widget.Aux = model.WidgetGroupTypeId.ToString();
                var diagram = new DynamicChart()
                {
                    Id = model.Id,
                    IsActive = model.IsActive,
                    DataSetting = model.DataSetting,
                    ReportId = model.ReportId,
                    WidgetTypeId = widget.Id,
                    CreatorId = user.StaffId
                };
                DbContext.DynamicCharts.Update(diagram);
            }
        }
    }

    public IEnumerable<DynamicChartViewModel> GetAllDynamicChart()
    {
        return DbContext.DynamicCharts.Select(d => new DynamicChartViewModel()
        {
            Id = d.Id,
            Report = d.Report.Title,
            IsActive = d.IsActive,
            WidgetType = d.WidgetType.Title,
            Creator = d.Creator.FName + " " + d.Creator.LName
        });
    }
    private IQueryable<Guid> GetRoleMapPostTypeAccessId(Guid staffId)
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

    private IQueryable<Guid> GetRoleMapPostTitleAccessId(Guid staffId)
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
    public IEnumerable<DynamicChartViewModel> GetByAccessPolicy(string username)
    {
        var user = DbContext.Users.Single(c => c.UserName == username);
        var userId = user.Id;
        var staffId = user.StaffId;

        var roleAccessesId = from ra in DbContext.RoleAccesses
                             where ra.UserId == userId
                             select ra.RoleId;

        var roleMapChartIds = from organizationInfo in DbContext.OrganiztionInfos
                              join chart in DbContext.Charts on organizationInfo.ChartId equals chart.Id
                              join roleMapChart in DbContext.RoleMapCharts on chart.Id equals roleMapChart.ChartId
                              where organizationInfo.StaffId == staffId
                              select roleMapChart.RoleId;

        var roleMapPostTypeAccessId = GetRoleMapPostTypeAccessId(staffId);

        var roleMapPostTitleAccessId = GetRoleMapPostTitleAccessId(staffId);

        var roleIds = new List<Guid>();
        roleIds.AddRange(roleMapPostTitleAccessId);
        roleIds.AddRange(roleMapChartIds);
        roleIds.AddRange(roleAccessesId);
        roleIds.AddRange(roleMapPostTypeAccessId);
        roleIds = roleIds.Distinct().ToList();

        var checkDynamicChartInAccess = from dc in DbContext.DynamicCharts
                                        join roleClaim in DbContext.RoleClaims on dc.Id.ToString() equals roleClaim.ClaimValue
                                        join roleId in roleIds on roleClaim.RoleId equals roleId
                                        where roleClaim.ClaimType == PermissionPolicyType.DynamicChartReportPermission
                                        select new DynamicChartViewModel()
                                        {
                                            Id = dc.Id,
                                            Report = dc.Report.Title,
                                            IsActive = dc.IsActive,
                                            WidgetType = dc.WidgetType.Title,
                                            WidgetTypeId = dc.WidgetTypeId,
                                            Creator = dc.Creator.FName + " " + dc.Creator.LName
                                        };

        var checkDynamicChartForPerson = from dc in DbContext.DynamicCharts.ToList()
                                         where dc.CreatorId == staffId
                                         select new DynamicChartViewModel()
                                         {
                                             Id = dc.Id,
                                             Report = dc.Report.Title,
                                             IsActive = dc.IsActive,
                                             WidgetType = dc.WidgetType.Title,
                                             WidgetTypeId = dc.WidgetTypeId,
                                             Creator = dc.Creator.FName + " " + dc.Creator.LName
                                         };

        var all = checkDynamicChartForPerson.Union(checkDynamicChartInAccess);

        return all.DistinctBy(d => d.Id).ToList();
    }

    public IEnumerable<DynamicChartViewModel> GetByAccess(Guid staffId)
    {
        var userId = DbContext.Users.Single(d => d.StaffId == staffId).Id;

        var access = from user in DbContext.Users
                     join roleAccess in DbContext.RoleAccesses on user.Id equals roleAccess.UserId
                     join role in DbContext.Roles on roleAccess.RoleId equals role.Id
                     where role.Name == "sysadmin" && user.Id == userId
                     select role;
        if (access.Any())
        {
            return GetAllDynamicChart();
        }
        var roleAccessesId = from ra in DbContext.RoleAccesses
                             where ra.UserId == userId
                             select ra.RoleId;

        var rolemapchartaccessId = from organiztionInfo in DbContext.OrganiztionInfos
                                   join chart in DbContext.Charts on organiztionInfo.ChartId equals chart.Id
                                   join rolemapchar in DbContext.RoleMapCharts on chart.Id equals rolemapchar.ChartId
                                   where organiztionInfo.StaffId == staffId
                                   select rolemapchar.RoleId;

        var roleMapPostTypeAccessId = GetRoleMapPostTypeAccessId(staffId);

        var roleMapPostTitleAccessId = GetRoleMapPostTitleAccessId(staffId);

        var roleIds = new List<Guid>();
        roleIds.AddRange(roleMapPostTitleAccessId);
        roleIds.AddRange(rolemapchartaccessId);
        roleIds.AddRange(roleAccessesId);
        roleIds.AddRange(roleMapPostTypeAccessId);
        roleIds = roleIds.Distinct().ToList();

        var checkDynamicChartInAccess = from dc in DbContext.DynamicCharts.ToList()
                                        join roleClaim in DbContext.RoleClaims on dc.Id.ToString() equals roleClaim.ClaimValue
                                        join roleId in roleIds on roleClaim.RoleId equals roleId
                                        where roleClaim.ClaimType == PermissionPolicyType.DynamicChartReportPermission
                                        select new DynamicChartViewModel()
                                        {
                                            Id = dc.Id,
                                            Report = dc.Report.Title,
                                            IsActive = dc.IsActive,
                                            WidgetType = dc.WidgetType.Title,
                                            WidgetTypeId = dc.WidgetTypeId,
                                            Creator = dc.Creator.FName + " " + dc.Creator.LName
                                        };

        var checkDynamicChartForPerson = from dc in DbContext.DynamicCharts.ToList()
                                         where dc.CreatorId == staffId
                                         select new DynamicChartViewModel()
                                         {
                                             Id = dc.Id,
                                             Report = dc.Report.Title,
                                             IsActive = dc.IsActive,
                                             WidgetType = dc.WidgetType.Title,
                                             WidgetTypeId = dc.WidgetTypeId,
                                             Creator = dc.Creator.FName + " " + dc.Creator.LName
                                         };
        var all = checkDynamicChartForPerson.Union(checkDynamicChartInAccess);
        return all.DistinctBy(d => d.Id).ToList();
    }

    public DynamicChartViewModel NewDynamicChart()
    {
        var look = DbContext.LookUps.Where(l => l.Type == "WidgetGroup" && l.IsActive).ToList();
        var listItemLook = new List<SelectListItem>();
        var listItemReport = new List<SelectListItem>();

        foreach (var item in look)
        {
            listItemLook.Add(new SelectListItem()
            {
                Text = item.Title,
                Value = item.Id.ToString()
            });
        }

        var report = DbContext.Reports.ToList();
        foreach (var item in report)
        {
            listItemReport.Add(new SelectListItem()
            {
                Text = item.Title,
                Value = item.Id.ToString()
            });
        }
        var dia = new DynamicChartViewModel()
        {
            Id = Guid.NewGuid(),
            ReportListItem = listItemReport,
            WidgetGroupTypeListItem = listItemLook
        };
        return dia;
    }

    public void DeleteChart(Guid id)
    {
        var chart = DbContext.DynamicCharts.Find(id);
        if (chart == null)
        {
            throw new ArgumentException("رکورد مورد نظر یافت نشد.");
        }

        DbContext.DynamicCharts.Remove(chart);

        var widget = DbContext.LookUps.Single(l => l.Id == chart.WidgetTypeId);
        DbContext.LookUps.Remove(widget);
    }

    public DynamicChartViewModel GetById(Guid? id)
    {
        var look = DbContext.LookUps.Where(l => l.Type == "WidgetGroup" && l.IsActive).ToList();
        var listItemLook = new List<SelectListItem>();
        var listItemReport = new List<SelectListItem>();

        foreach (var item in look)
        {
            listItemLook.Add(new SelectListItem()
            {
                Text = item.Title,
                Value = item.Id.ToString()
            });
        }

        var report = DbContext.Reports.ToList();
        foreach (var item in report)
        {
            listItemReport.Add(new SelectListItem()
            {
                Text = item.Title,
                Value = item.Id.ToString()
            });
        }
        var chart = DbContext.DynamicCharts.Find(id);
        var widget = DbContext.LookUps.Find(chart.WidgetTypeId);
        var thisReport = DbContext.Reports.SingleOrDefault(c => c.Id == chart.ReportId);

        var vm = new DynamicChartViewModel()
        {
            IsActive = chart.IsActive,
            DataSetting = chart.DataSetting,
            ReportId = chart.ReportId,
            ReportListItem = listItemReport,
            WidgetGroupTypeListItem = listItemLook,
            WidgetType = chart.WidgetType.Title,
            WidgetTypeId = chart.WidgetTypeId,
            Id = chart.Id,
            WidgetGroupTypeId = Guid.Parse(widget.Aux),
            IsEdit = true,
            Report = thisReport.Title
        };
        return vm;
    }

    public DynamicChartViewModel GetByWidgetId(Guid id)
    {
        var chart = DbContext.DynamicCharts.Single(d => d.WidgetTypeId == id);
        var vm = new DynamicChartViewModel()
        {
            IsActive = chart.IsActive,
            DataSetting = chart.DataSetting,
            ReportId = chart.ReportId,
            WidgetType = chart.WidgetType.Title,
            WidgetTypeId = chart.WidgetTypeId,
            Id = chart.Id
        };
        return vm;
    }
}