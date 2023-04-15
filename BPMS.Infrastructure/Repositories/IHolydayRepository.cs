using BPMS.Domain.Common.ViewModels;
using BPMS.Domain.Common.ViewModels.Global;
using BPMS.Domain.Entities;
using BPMS.Infrastructure.Repositories.Base;

namespace BPMS.Infrastructure.Repositories;

public interface IHolydayRepository : IRepository<Holyday>
{
    HolydayViewModel CreateNewHolyday();
    IEnumerable<HolydayViewModel> GetHolydayDay();
    Result SaveHolydayRecord(HolydayViewModel model);
    void SaveWorkTime(HolydayViewModel model);
    List<Holyday> GetHolidays();
}