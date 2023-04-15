namespace BPMS.Domain.Common.ViewModels;

public class ExternalFlowsInCartbotViewModel
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
    public string IsOnline { get; set; }
    public Guid WorkflowDetailId { get; set; }
    public string ImagePath { get; set; }
}