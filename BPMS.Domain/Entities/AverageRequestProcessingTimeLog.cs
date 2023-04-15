namespace BPMS.Domain.Entities;

public class AverageRequestProcessingTimeLog
{
    public AverageRequestProcessingTimeLog()
    {
    }

    private AverageRequestProcessingTimeLog(Guid id, Guid workFlowId, string? processName, int? avgTimeToDo, int? avgTimeDone, int? min, int? max,
        int count, DateTime createdDate, TimeSpan createdTime, DateTime? updatedDate, TimeSpan? updatedTime)
    {
        Id = id;
        WorkFlowId = workFlowId;
        ProcessName = processName;
        AvgTimeToDo = avgTimeToDo;
        AvgTimeDone = avgTimeDone;
        Min = min;
        Max = max;
        Count = count;
        CreatedDate = createdDate;
        CreatedTime = createdTime;
        UpdatedDate = updatedDate;
        UpdatedTime = updatedTime;
    }

    public static AverageRequestProcessingTimeLog CreatedNew(Guid workFlowId, string? processName, int? avgTimeToDo, int? avgTimeDone, int? min, int? max, int count)
    {
        return new AverageRequestProcessingTimeLog(
            Guid.NewGuid(),
            workFlowId,
            processName,
            avgTimeToDo,
            avgTimeDone,
            min,
            max,
            count,
            DateTime.Now.Date,
            DateTime.Now.TimeOfDay,
            null,
            null);
    }

    public void UpdateEntity(Guid workFlowId, string? processName, int? avgTimeToDo, int? avgTimeDone, int? min, int? max, int count)
    {
        WorkFlowId = workFlowId;
        ProcessName = processName;
        AvgTimeToDo = avgTimeToDo;
        AvgTimeDone = avgTimeDone;
        Min = min;
        Max = max;
        Count = count;
        UpdatedDate = DateTime.Now.Date;
        UpdatedTime = DateTime.Now.TimeOfDay;
    }

    public Guid Id { get; set; }
    public Guid WorkFlowId { get; private set; }
    public string? ProcessName { get; private set; }
    public int? AvgTimeToDo { get; private set; }
    public int? AvgTimeDone { get; private set; }
    public int? Min { get; private set; }
    public int? Max { get; private set; }
    public int Count { get; private set; }

    public DateTime CreatedDate { get; private set;}
    public TimeSpan CreatedTime { get; private set; }

    public DateTime? UpdatedDate { get; private set; }
    public TimeSpan? UpdatedTime { get; private set; }
}