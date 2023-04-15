using BPMS.Domain.Common.Dtos;

namespace BPMS.Infrastructure.Services;

public interface IScheduleLogsService
{
    IEnumerable<ScheduleLogDTO> GetScheduleLogsByScheduleId(Guid idSchedule);
    IEnumerable<ScheduleLogDTO> GetAllScheduleLogs();
    void AddScheduleLog(Guid id);
    DateTime? GetScheduleLastLogDate(Guid id);
}