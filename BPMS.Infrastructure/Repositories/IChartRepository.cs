using BPMS.Domain.Common.Dtos;
using BPMS.Domain.Common.ViewModels;
using BPMS.Domain.Entities;
using Kendo.Mvc.UI;
using BPMS.Infrastructure.Repositories.Base;

namespace BPMS.Infrastructure.Repositories;

public interface IChartRepository : IRepository<Chart>
{
    IEnumerable<Chart> GetActiveCharts();
    void MapChart(List<ChartViewModel> model);
    IEnumerable<ChartDto> GetAllCharts();
    List<TreeViewItemModel> GetChartInTree(Guid? id, List<Guid> ids);
    void UpdateModifeidChart(ChartPhpDto model);
    List<ChartDiagramViewModel> GetChartForDiagram(Guid? id);
    List<Chart> GetSubCharts(Guid id);
    List<Guid> GetChartStaffsId(Guid chartId);
    IQueryable<string> GetEmailsByChartId(Guid chartId);
    IQueryable<string> GetPhoneNumbersByChartId(Guid chartId);
}