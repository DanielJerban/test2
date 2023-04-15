using BPMS.Domain.Common.Enums;

namespace BPMS.Domain.Entities;

public class ScheduleLifeTimeLog
{
    public ScheduleLifeTimeLog()
    {

    }

    public ScheduleLifeTimeLog(Guid id, ScheduleType scheduleType, DateTime createdDate, TimeSpan startTime, TimeSpan endTime, TimeSpan elapsed, bool success, string errorMessage)
    {
        Id = id;
        ScheduleType = scheduleType;
        CreatedDate = createdDate;
        StartTime = startTime;
        EndTime = endTime;
        Elapsed = elapsed;
        Success = success;
        ErrorMessage = errorMessage;
    }

    public static ScheduleLifeTimeLog CreateNew(ScheduleType scheduleType, TimeSpan startTime, TimeSpan endTime, TimeSpan elapsed, bool success, string errorMessage)
        => new(Guid.NewGuid(), scheduleType, DateTime.Now, startTime, endTime, elapsed, success, errorMessage);

    public Guid Id { get; set; }
    public ScheduleType ScheduleType { get; set; }
    public DateTime CreatedDate { get; set; }
    public TimeSpan StartTime { get; set; }
    public TimeSpan EndTime { get; set; }
    public TimeSpan Elapsed { get; set; }
    public bool Success { get; set; }
    public string? ErrorMessage { get; set; }
}