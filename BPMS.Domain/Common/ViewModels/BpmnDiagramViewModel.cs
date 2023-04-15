using BPMS.Domain.Common.Dtos;
using Kendo.Mvc.UI;
using Microsoft.AspNetCore.Http;

namespace BPMS.Domain.Common.ViewModels;

public class BpmnDiagramViewModel
{
    public List<SelectListItem> RequestTypeListItem { get; set; }
    public IEnumerable<SelectListItem> RequestGroupTypeListItem { get; set; }
    public IEnumerable<SelectListItem> StaffListItem { get; set; }
    public IEnumerable<SelectListItem> FlowTypeListItem { get; set; }
    public WorkFlowViewModel Workflow { get; set; }
    public IEnumerable<TreeViewItemModel> Charts { get; set; }
    public string TutorialFileName { get; set; }
    public IFormFile? File { get; set; }
}