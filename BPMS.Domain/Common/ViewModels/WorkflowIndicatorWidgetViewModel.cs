namespace BPMS.Domain.Common.ViewModels;

public class WorkflowIndicatorWidgetViewModel
{
    public string RequestTypeTitle { get; set; }
    public string Activity { get; set; }
    public string WidgetType { get; set; }
    public Guid WorkflowIndicatorId { get; set; }
    public Guid WorkflowId { get; set; }
    public Guid RequestTypeId { get; set; }
    public Guid WorkflowDetailId { get; set; }
    public string FlowStatus { get; set; }
    public int Count { get; set; }
    public int Duration { get; set; }
    public int MaxValue { get; set; }
    public int MinValue { get; set; }
    public int Crisis { get; set; }
    public int Warning { get; set; }
    public int CalcCriterionCode { get; set; }
}