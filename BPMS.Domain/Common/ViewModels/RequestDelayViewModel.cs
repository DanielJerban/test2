using BPMS.Domain.Entities;

namespace BPMS.Domain.Common.ViewModels;

public class RequestDelayViewModel
{
    public Guid FlowId { get; set; }
    public long RequestNo { get; set; }
    public string FullName { get; set; }
    public string PersonalCode { get; set; }
    public string RequestTypeTitle { get; set; }
    public string WorkflowName { get; set; }
    public string WorkFlowVersion { get; set; }
    public string SubprocessName { get; set; }
    public string WorkflowNameAndVersion { get; set; }
    public string StepTitle { get; set; }
    public string RequestDate { get; set; }
    public string RequestTime { get; set; }
    public string CurrentStatus { get; set; }
    public Guid RequestId { get; set; }
    public Guid RequestTypeId { get; set; }
    public Guid WorkflowDetailId { get; set; }
    public Guid WorkflowId { get; set; }
    public Guid StaffId { get; set; }
    public string TimeToDo { get; set; }
    public string Delay { get; set; }
    public string ImagePath { get; set; }
    public int? DelayDate { get; set; }
    public string DelayTime { get; set; }
    public int? WaitingTime { get; set; }
    public Workflow Workflow { get; set; }

    // WorkflowName or subprocess name
    public string Name { get; set; }
    /// <summary>
    /// نام دریافت کننده دزخواست
    /// </summary>
    public string PersonalName { get; set; }
}