namespace BPMS.Domain.Common.ViewModels;

public class CartbotAlertViewModel
{
    public string RequestNo { get; set; }
    public string RequestTypeTitle { get; set; }
    public string WorkFlowDetailTitle { get; set; }
    public Guid RequestId { get; set; }
    public Guid FlowId { get; set; }
    public string FlowStatus { get; set; }
    public string FlowStatusCode { get; set; }

}