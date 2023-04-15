using BPMS.Domain.Common.Enums;

namespace BPMS.Domain.Entities;

public class ScheduleLog
{
    public ScheduleLog()
    {

    }
    private ScheduleLog(Guid id, ScheduleType scheduleType, DateTime startedAt, DateTime endedAt, bool isSuccess, string errorMessage)
    {
        Id = id;
        ScheduleType = scheduleType;
        StartedAt = startedAt;
        EndedAt = endedAt;
        IsSuccess = isSuccess;
        ErrorMessage = errorMessage;
    }

    public static ScheduleLog CreateNew(ScheduleType scheduleType, DateTime startedAt, DateTime endedAt, bool isSuccess, string errorMessage)
        => new ScheduleLog(Guid.NewGuid(), scheduleType, startedAt, endedAt, isSuccess, errorMessage);

    public Guid Id { get; set; }
    public ScheduleType ScheduleType { get; set; }
    public DateTime StartedAt { get; set; }
    public DateTime EndedAt { get; set; }
    public bool IsSuccess { get; set; }
    public string? ErrorMessage { get; set; }
}