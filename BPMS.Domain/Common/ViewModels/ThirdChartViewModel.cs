using BPMS.Domain.Entities;

namespace BPMS.Domain.Common.ViewModels;

public class ThirdChartViewModel
{
    public string Title { get; set; }
    public int Count { get; set; }
    public long ReqNo { get; set; }
    public Guid ReqTypeId { get; set; }
    public double DelayTime { get; set; }
    public string StaffFullName { get; set; }
    public string WorkflowDetailTitle { get; set; }
    public string RequestStatus { get; set; }
    public Guid WorkflowDetailId { get; set; }
}

public class RequestsWithDelayViewModel
{
    public double DelayTime { get; set; }
    public Request RequestDetail { get; set; }
    public string ConfirmPerson { get; set; }
    public string WorkflowDetailTitle { get; set; }
    public int Counter { get; set; }
    public Guid WorkflowDetailId { get; set; }
}