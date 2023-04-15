using BPMS.Application.Repositories.Base;
using BPMS.Domain.Common.Enums;
using BPMS.Domain.Common.Helpers;
using BPMS.Domain.Entities;
using BPMS.Infrastructure.Data;
using BPMS.Infrastructure.Repositories;

namespace BPMS.Application.Repositories;

public class ScheduleLifeTimeLogRepository : Repository<ScheduleLifeTimeLog> , IScheduleLifeTimeLogRepository
{
    public ScheduleLifeTimeLogRepository(BpmsDbContext context) : base(context)
    {
    }

    public string GetLastRunTime(ScheduleType scheduleType)
    {
        var lastUpdateDateString = "";
        var lastUpdateDate = Context.ScheduleLifeTimeLogs
            .Where(c => c.Success && c.ScheduleType == scheduleType)
            .OrderByDescending(c => c.CreatedDate).FirstOrDefault();

        if (lastUpdateDate != null)
        {
            var x = HelperBs.ConvertMiladyToIntShamsi(lastUpdateDate.CreatedDate);
            var dateString = x.ToString().Insert(4, "/").Insert(7, "/") + " - " + lastUpdateDate.CreatedDate.ToShortTimeString();
            lastUpdateDateString = dateString;
        }

        return lastUpdateDateString;
    }
}