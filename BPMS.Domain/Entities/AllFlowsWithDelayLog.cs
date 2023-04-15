using System.ComponentModel.DataAnnotations;

namespace BPMS.Domain.Entities;

public class AllFlowsWithDelayLog
{
    [Key]
    public Guid FlowId { get; set; }
    public Guid WorkflowId { get; set; }
    public Guid FlowStaffId { get; set; }
    public Guid RequestStaffId { get; set; }
    public string? FlowStatus { get; set; }
    public string? RequestStatus { get; set; }
    public string? FlowStaff { get; set; }
    public long RequestNo { get; set; }
    
    [DataType(DataType.DateTime)]
    public DateTime RegisterDate { get; set; }
    public string? ProcessName { get; set; }
    public string? SubProcessName { get; set; }
    public string? WorkflowDetailTitle { get; set; }
    public string? RequestStaff { get; set; }

    [DataType(DataType.DateTime)]
    public DateTime Deadline { get; set; }

    public int PredictedTime { get; set; }
    public int? TimeTodo { get; set; }
    public int DelayHour { get; set; }


    public int PredictedTimeAllToThisStep { get; set; }
    public int TimeTodoAllToThisStep { get; set; }
    public int DelayHourToThisStep { get; set; }
}