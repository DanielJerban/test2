using BPMS.Domain.Common.ViewModels;
using BPMS.Domain.Entities;
using BPMS.Infrastructure.Repositories.Base;

namespace BPMS.Infrastructure.Repositories;

public interface IScheduleRepository : IRepository<Schedule>
{
    IEnumerable<ScheduleViewModel> GetScheduleRecords();
    void CreateSchedules(ScheduleViewModel model);
    List<Schedule> GetActiveSchedules();
    //void GenerateScheduleLog(Guid ScheduelId);
}