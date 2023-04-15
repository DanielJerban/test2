namespace BPMS.Domain.Common.ViewModels;

public class RequestDelayGridViewModel
{
    public string PersonalName { get; set; }
    public string RequestNumber { get; set; }
    public string RequestTypeTitle { get; set; }
    public string FlowNameAndVersion { get; set; }
    public string SubprocessName { get; set; }
    public string FlowName { get; set; }
    public string FlowLevelName { get; set; }
    public string ApplicantName { get; set; }
    public string RequestType { get; set; }
    public double? DelayHour { get; set; }
    public Guid RequestId { get; set; }
    public string RequestDate { get; set; }
    public string RequestTime { get; set; }
    public string RequestDateTime { get; set; }
    public string TimeToDo { get; set; }
    public Guid? FlowId { get; set; }
    public Guid? WorkFlowDetailId { get; set; }
}