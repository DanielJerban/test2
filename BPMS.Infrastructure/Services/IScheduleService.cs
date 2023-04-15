using BPMS.Domain.Entities;

namespace BPMS.Infrastructure.Services;

public interface IScheduleService
{
    void ExecuteSchedules(Schedule schedule);
    List<Schedule> GetSchedules();
    void ResetCatch();
}