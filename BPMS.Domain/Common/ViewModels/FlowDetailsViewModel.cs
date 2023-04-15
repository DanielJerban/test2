using System.ComponentModel;

namespace BPMS.Domain.Common.ViewModels;

public class ShowFlowDetailsViewModel
{
    public IEnumerable<FlowDetailsViewModel> FlowDetails { get; set; }
    public FlowDetailsInfo DetailsInfo { get; set; }
}

public class FlowDetailsInfo
{
    public string RequestNo { get; set; }
    public string Requestor { get; set; }
    public string ReqDate { get; set; }
    public string ReqTime { get; set; }
    public  string ReqStatus { get; set; }
    public Guid? WorkflowDetailId { get; set; }
    public Guid? ActiveWorkflowDetailId { get; set; }
    public Guid RequestId { get; set; }
    public int ReqStatusCode { get; set; }
    public string Building { get; set; }
    public string LocalPhone { get; set; }
    public string PostTitle { get; set; }
    public string CompanyName { get; set; }
}
public class FlowDetailsViewModel
{
    public string Row { get; set; }
    public string TimeToDo { get; set; }
    public int? DelayDate { get; set; }
    public string DelayTime { get; set; }
    public int? WaitingTime { get; set; }

    public string Delay { get; set; }
    public long RequestNo { get; set; }
    public string Status { get; set; }
    public string FullName { get; set; }

    public string ResponeDate { get; set; }
    public string ResponeTime { get; set; }
    public string Dsr { get; set; }
    [DisplayName("شماره داخلی")]
    public string LocalPhone { get; set; }
    [DisplayName("عنوان مرحله")]
    public string StepTitle { get; set; }
    [DisplayName("ساختمان")]
    public string Building { get; set; }

}