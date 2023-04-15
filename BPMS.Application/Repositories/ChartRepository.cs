using BPMS.Application.Repositories.Base;
using BPMS.Domain.Common.Dtos;
using BPMS.Domain.Common.ViewModels;
using BPMS.Domain.Entities;
using BPMS.Infrastructure.Data;
using BPMS.Infrastructure.Repositories;
using Kendo.Mvc.UI;
using Microsoft.EntityFrameworkCore;

namespace BPMS.Application.Repositories;

public class ChartRepository : Repository<Chart>, IChartRepository
{
    public ChartRepository(BpmsDbContext context) : base(context)
    {
    }

    public BpmsDbContext DbContext => Context;

    public IEnumerable<Chart> GetActiveCharts()
    {
        return DbContext.Charts.Where(p => p.IsActive).ToList();
    }

    public IEnumerable<ChartDto> GetAllCharts()
    {
        return DbContext.Charts.Include(c => c.ChartLevel).Select(s => new ChartDto()
        {
            Id = s.Id,
            Title = s.Title,
            IsActive = s.IsActive,
            ParentId = s.ParentId,
            ChartLevelId = s.ChartLevelId,
            ChartLevel = s.ChartLevel.Title
        });
    }

    public List<ChartDiagramViewModel> GetChartForDiagram(Guid? id)
    {
        var charts = new List<ChartDiagramViewModel>();
        var root = DbContext.Charts.Where(p => p.ParentId == id).ToList();
        foreach (var node in root)
        {
            var staff = (from chart in DbContext.Charts
                         join org in DbContext.OrganiztionInfos on chart.Id equals org.ChartId
                         join user in DbContext.Staffs on org.StaffId equals user.Id
                         let title = DbContext.LookUps.FirstOrDefault(d => d.Id == org.OrganiztionPostId)
                         let type = DbContext.LookUps.FirstOrDefault(r => r.Id.ToString() == title.Aux)
                         where chart.Id == node.Id && type.Aux2 == "1"
                         select user).FirstOrDefault();

            var newRec = new ChartDiagramViewModel()
            {
                FirstName = staff?.FName,
                Title = node.Title,
                LastName = staff?.LName,
                PersonalCode = staff?.PersonalCode,
                Items = GetChartForDiagram(node.Id),
                ColorScheme = "#1696d3",
                ChartId = node.Id
            };
            charts.Add(newRec);
        }

        return charts;
    }

    public List<TreeViewItemModel> GetChartInTree(Guid? id, List<Guid> ids)
    {
        var chart = new List<TreeViewItemModel>();
        var root = DbContext.Charts.Where(p => p.ParentId == id && p.IsActive).ToList();
        foreach (var node in root)
        {
            bool check = false;
            if (ids != null)
            {
                check = ids.Any(d => d == node.Id);
            }

            chart.Add(new TreeViewItemModel()
            {
                Text = node.Title,
                Id = node.Id.ToString(),
                Items = GetChartInTree(node.Id, ids),
                Checked = check
            });

        }

        return chart;
    }

    public void MapChart(List<ChartViewModel> model)
    {
        var chartUpdate = DbContext.Charts.ToList()
            .Where(c => model.Any(u => u.Id == c.Id));
        foreach (var chart in chartUpdate)
        {
            var myChart = model.Single(m => m.Id == chart.Id);
            chart.Id = myChart.Id;
            chart.ParentId = myChart.ParentId;
            chart.ChartLevelId = myChart.ChartLevelId;
            chart.Title = myChart.Title;
            chart.IsActive = myChart.IsActive;
        }

        var chartInsert = from chart in model
                          where DbContext.Charts.ToList().All(w => w.Id != chart.Id)
                          select chart;
        foreach (var chartViewModel in chartInsert)
        {
            DbContext.Charts.Add(new Chart()
            {
                Id = chartViewModel.Id,
                ParentId = chartViewModel.ParentId,
                ChartLevelId = chartViewModel.ChartLevelId,
                IsActive = chartViewModel.IsActive,
                Title = chartViewModel.Title
            });
        }

        var chartRemove = DbContext.Charts.ToList()
            .Where(c => model.All(u => u.Id != c.Id)).ToList();


        DbContext.Charts.RemoveRange(chartRemove);
    }

    public void UpdateModifeidChart(ChartPhpDto model)
    {
        switch (model.Action)
        {
            case "Insert":
                {
                    var code = int.Parse(model.ChartLevelId);
                    var parentid = int.Parse(model.ParentId);
                    var chartLevel = DbContext.LookUps.FirstOrDefault(l => l.Type == "ChartLevel" && l.Code == code && l.IsActive);
                    if (chartLevel == null)
                        throw new ArgumentException("سطح چارت وجود ندارد.");
                    var parent = DbContext.Charts.FirstOrDefault(c => c.PhpId == parentid && c.IsActive);
                    if (parent == null)
                        // return;
                        throw new ArgumentException("پرنت چارت " + model.PhpId + " وجود ندارد.");

                    var chart = new Chart()
                    {
                        IsActive = model.IsActive == "active",
                        ChartLevelId = chartLevel.Id,
                        ParentId = parent.Id,
                        Title = model.Title,
                        PhpId = model.PhpId
                    };
                    DbContext.Charts.Add(chart);
                    break;
                }
            case "Update":
                {
                    var chart = DbContext.Charts.FirstOrDefault(c => c.PhpId == model.PhpId);
                    if (chart == null)
                    {
                        throw new ArgumentException("چارت پیدا نشد.");
                    }
                    if (!string.IsNullOrWhiteSpace(model.IsActive))
                    {
                        chart.IsActive = model.IsActive == "active";
                    }

                    if (!string.IsNullOrWhiteSpace(model.Title))
                    {
                        chart.Title = model.Title;
                    }

                    if (!string.IsNullOrWhiteSpace(model.ChartLevelId))
                    {
                        var code = int.Parse(model.ChartLevelId);
                        var chartLevel = DbContext.LookUps.FirstOrDefault(l => l.Type == "ChartLevel" && l.Code == code && l.IsActive);
                        if (chartLevel == null)
                            throw new ArgumentException("سطح چارت وجود ندارد.");
                        chart.ChartLevelId = chartLevel.Id;
                    }

                    if (!string.IsNullOrWhiteSpace(model.ParentId))
                    {
                        var parentid = int.Parse(model.ParentId);
                        var parent = DbContext.Charts.FirstOrDefault(c => c.PhpId == parentid);
                        if (parent == null)
                            //  return;
                            throw new ArgumentException("پرنت چارت " + model.PhpId + " وجود ندارد. شناسه پرنت: " + model.ParentId);
                        chart.ParentId = parent.Id;
                    }
                    DbContext.Charts.Update(chart);
                    break;
                }
            case "Delete":
                {

                    var chart = DbContext.Charts.FirstOrDefault(s => s.PhpId == model.PhpId);
                    if (chart == null)
                    {
                        throw new ArgumentException("چارت پیدا نشد.");
                    }
                    var org = DbContext.OrganiztionInfos.Where(o => o.ChartId == chart.Id).ToList();
                    if (org.Any())
                    {
                        throw new ArgumentException("امکان حذف این رکورد به دلیل استفاده شدن در پست سازمانی وجود ندارد.");
                    }

                    var chartchild = DbContext.Charts.Where(c => c.ParentId == chart.Id).ToList();
                    if (chartchild.Any())
                    {
                        throw new ArgumentException("امکان حذف این رکورد به دلیل داشتن زیر مجموعه وجود ندارد.");
                    }
                    var rmc = DbContext.RoleMapCharts.Where(c => c.ChartId == chart.Id).ToList();
                    if (rmc.Any())
                    {
                        throw new ArgumentException("امکان حذف این رکورد به دلیل داشتن ارتباط با گروه دسترسی وجود ندارد.");
                    }
                    DbContext.Charts.Remove(chart);
                    break;
                }

            default:
                throw new ArgumentException("خطا در ارسال درخواست");
        }
    }

    public List<Chart> GetSubCharts(Guid id)
    {
        var charts = new List<Chart>();

        var chart = Context.Charts.SingleOrDefault(c => c.Id == id);

        var thisChartSubs = Context.Charts.Where(c => c.ParentId == chart.Id && c.IsActive).ToList();
        charts.Add(chart);

        if (!thisChartSubs.Any())
        {
            return charts;
        }

        foreach (var item in thisChartSubs)
        {
            var itemSubCharts = GetSubCharts(item.Id);
            charts.AddRange(itemSubCharts);
        }

        return charts;
    }

    public List<Guid> GetChartStaffsId(Guid chartId)
    {
        return Context.OrganiztionInfos.Where(c => c.ChartId == chartId).Select(c => c.StaffId).ToList();
    }

    public IQueryable<string> GetEmailsByChartId(Guid chartId)
    {
        return from chart in Context.Charts
               join org in DbContext.OrganiztionInfos on chart.Id equals org.ChartId
               join staff in DbContext.Staffs on org.StaffId equals staff.Id
               where chart.Id == chartId && staff.Email != null && staff.Email != ""
               select staff.Email;
    }

    public IQueryable<string> GetPhoneNumbersByChartId(Guid chartId)
    {
        return from chart in DbContext.Charts
               join org in DbContext.OrganiztionInfos on chart.Id equals org.ChartId
               join staff in DbContext.Staffs on org.StaffId equals staff.Id
               where chart.Id == chartId && staff.PhoneNumber != null && staff.PhoneNumber != ""
               select staff.PhoneNumber;
    }
}