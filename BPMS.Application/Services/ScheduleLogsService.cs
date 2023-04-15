using BPMS.Domain.Common.Dtos;
using BPMS.Domain.Common.Helpers;
using BPMS.Domain.Common.ViewModels;
using BPMS.Infrastructure.Repositories;
using BPMS.Infrastructure.Services;

namespace BPMS.Application.Services;

public class ScheduleLogsService : IScheduleLogsService
{
    private readonly IScheduleLogRepository _scheduleLogRepository;
    public ScheduleLogsService(IScheduleLogRepository scheduleLogRepository)
    {
        _scheduleLogRepository = scheduleLogRepository;
    }

    public void AddScheduleLog(Guid id)
    {
        var date = DateTime.Now;
        var time = date.ToString("HHmm");
        var dateNow = HelperBs.ConvertMiladyToIntShamsi(date);

        ScheduleLogViewModel scheduleLog = new ScheduleLogViewModel()
        {
            ScheduleId = id,
            RegisterDate = dateNow,
            RegisterTime = time,
            RunDate = dateNow,
            RunTime = time
        };
        _scheduleLogRepository.AddScheduleLog(scheduleLog);
    }

    public IEnumerable<ScheduleLogDTO> GetScheduleLogsByScheduleId(Guid idSchedule)
    {
        return _scheduleLogRepository.GetScheduleLogsByScheduleId().Where(x => x.ScheduleId == idSchedule).Select(x => new ScheduleLogDTO
        {
            RegisterDate = x.RegisterDate,
            RegisterTime = x.RegisterTime,
            RunDate = x.RunDate,
            RunTime = x.RunTime,
            Schedule = x.Schedule
        });

    }

    public DateTime? GetScheduleLastLogDate(Guid id)
    {
        var data = _scheduleLogRepository.GetScheduleLastLog(id);
        if (data.RegisterTime is null)
            return null;

        return HelperBs.ConvertShamsiToDateTime(data.RegisterDate.ToString(), data.RegisterTime);
    }

    public IEnumerable<ScheduleLogDTO> GetAllScheduleLogs()
    {
        return _scheduleLogRepository.GetAllScheduleLogs().Select(x => new ScheduleLogDTO
        {
            RegisterDate = x.RegisterDate,
            RegisterTime = x.RegisterTime,
            RunDate = x.RunDate,
            RunTime = x.RunTime,
            Schedule = x.Schedule,
            TaskType=x.TaskType
        });
    }
}