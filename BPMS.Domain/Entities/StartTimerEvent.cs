namespace BPMS.Domain.Entities;

public class StartTimerEvent
{
    public Guid Id { get; set; }
    public DateTime StartDateTime { get; set; }
    public bool IsSequential { get; set; }
    public int? IntervalHours { get; set; }
    public bool HasExpireDate { get; set; }
    public DateTime? ExpireDateTime { get; set; }
    public Guid WorkFlowId { get; set; }
    public Workflow Workflow { get; set; }
    public DateTime? LastRunTime { get; set; }
}