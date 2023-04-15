using BPMS.Domain.Common.ViewModels;
using BPMS.Domain.Entities;
using BPMS.Infrastructure.Repositories.Base;

namespace BPMS.Infrastructure.Repositories;

public interface IDynamicChartRepository : IRepository<DynamicChart>
{
    IEnumerable<DynamicChartViewModel> GetAllDynamicChart();
    void SaveChart(DynamicChartViewModel model, string username);
    DynamicChartViewModel NewDynamicChart();

    void DeleteChart(Guid id);
    DynamicChartViewModel GetById(Guid? id);
    DynamicChartViewModel GetByWidgetId(Guid id);
    IEnumerable<DynamicChartViewModel> GetByAccess(Guid userStaffId);
    IEnumerable<DynamicChartViewModel> GetByAccessPolicy(string username);
}