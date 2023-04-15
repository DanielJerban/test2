namespace BPMS.Domain.Common.ViewModels;

public class FlowsInCartbotViewModel
{
    public Guid FlowId { get; set; }
    public long RequestNo { get; set; }
    public string FullName { get; set; }
    public string PersonalCode { get; set; }
    public string RequestTypeTitle { get; set; }
    public string StepTitle { get; set; }
    public string RequestDate { get; set; }
    public string RequestTime { get; set; }
    public string CurrentStatus { get; set; }
    public Guid RequestId { get; set; }
    public Guid RequestTypeId { get; set; }
    public bool IsBalloon { get; set; }
    public Guid WorkflowDetailId { get; set; }
    public bool IsMultiConfirmReject { get; set; }
    public string Message { get; set; }
    public Guid StaffId { get; set; }
    public string TimeToDo { get; set; }
    public bool IsRed { get; set; }
    public bool IsRead { get; set; }
    public string Delay { get; set; }
    public string ImagePath { get; set; }
    public int? DelayDate { get; set; }
    public string DelayTime { get; set; }
    public string DelayDateTime { get; set; }
    public int? WaitingTime { get; set; }
    public int? ResponseDate { get; set; }
    public string ResponseTime { get; set; }
    public string ResponseDateTime { get; set; }
    public Guid WorkflowId { get; set; }
    public Guid? SubprocessId { get; set; }
    public int? DynamicWaitingTime { get; set; }
}