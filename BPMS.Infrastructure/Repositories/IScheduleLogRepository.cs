using BPMS.Domain.Common.ViewModels;
using BPMS.Domain.Entities;
using BPMS.Infrastructure.Repositories.Base;

namespace BPMS.Infrastructure.Repositories;

public interface IScheduleLogRepository : IRepository<ScheduleLog>
{
    string GetLastRunTimeDate(int code);
    void AddScheduleLog(ScheduleLogViewModel model);
    IEnumerable<GetScheduleLogViewModel> GetScheduleLogsByScheduleId();
    IEnumerable<GetScheduleLogViewModel> GetAllScheduleLogs();
    ScheduleLastLogDateViewModel GetScheduleLastLog(Guid id);
}