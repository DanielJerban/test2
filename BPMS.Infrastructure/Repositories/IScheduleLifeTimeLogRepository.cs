using BPMS.Domain.Common.Enums;
using BPMS.Domain.Entities;
using BPMS.Infrastructure.Repositories.Base;

namespace BPMS.Infrastructure.Repositories;

public interface IScheduleLifeTimeLogRepository : IRepository<ScheduleLifeTimeLog>
{
    string GetLastRunTime(ScheduleType scheduleType);
}