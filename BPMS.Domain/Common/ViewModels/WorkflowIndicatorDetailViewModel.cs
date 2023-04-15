using BPMS.Domain.Common.Dtos;

namespace BPMS.Domain.Common.ViewModels;

public class WorkflowIndicatorDetailViewModel
{
    public string FirstRequestDate { get; set; }
    public string LastRequestDate { get; set; }
    public string FirstRequestTime { get; set; }
    public string LastRequestTime { get; set; }
    public int FinishedRejectedRequestsCount { get; set; }
    public int FinishedAcceptedRequestsCount { get; set; }
    public int InProgressRequestsCount { get; set; }
    public int NotStartedRequestsCount { get; set; }
    public int TotalRequestsCount { get; set; }
    public IEnumerable<WorkflowIndicatorWidgetViewModel> Gauges { get; set; }
    public IEnumerable<ActivityStatusViewModel> InprogressActivity { get; set; }
    public IEnumerable<ActivityStatusViewModel> FinishdAcceptActivity { get; set; }
    public IEnumerable<ActivityStatusViewModel> FinishedRejectsActivity { get; set; }
    public IEnumerable<ActivityStatusViewModel> TotalActivity { get; set; }

    public IEnumerable<SelectListItem> Versions { get; set; }
    public string CurrentVersion { get; set; }
}