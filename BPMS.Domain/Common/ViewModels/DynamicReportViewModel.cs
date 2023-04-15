using BPMS.Domain.Common.Dtos;

namespace BPMS.Domain.Common.ViewModels;

public class DynamicReportViewModel
{
    public IEnumerable<SelectListItem> ReportListItem { get; set; }
    public IEnumerable<SelectListItem> RequestTypeItems { get; set; }
    public IEnumerable<SelectListItem> WorkflowItems { get; set; }

}