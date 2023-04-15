using BPMS.Domain.Entities;
using Kendo.Mvc.UI;

namespace BPMS.Domain.Common.ViewModels;

public class AccessViewModel
{
    public Role Role { get; set; }

    public IEnumerable<User> Users { get; set; }
    public IEnumerable<Chart> Charts { get; set; }
    public IEnumerable<LookUp> PostTypes { get; set; }
    public IEnumerable<LookUp> PostTitles { get; set; }
    public IEnumerable<Workflow> Workflows { get; set; }
    public IEnumerable<LookUp> ProcessBpmn { get; set; }
    public IEnumerable<LookUp> ProcessIndicator { get; set; }
    public IEnumerable<Entities.Report> Reports { get; set; }
    public IEnumerable<DynamicChart> DynamicCharts { get; set; }
    public IEnumerable<WorkFlowForm> WorkFlowForms { get; set; }
    public IEnumerable<WorkFlowFormList> WorkflowFormLists { get; set; }
    public IEnumerable<LookUp> VirtualTable { get; set; }
    public IEnumerable<LookUp> BpmnChangeButton { get; set; }
    public IEnumerable<Entities.Report> ReportsGenerated { get; set; }
    public IEnumerable<LookUp> ProcessStatus { get; set; }
    public IEnumerable<string> ActionRoles { get; set; }
    public IEnumerable<Guid> WidgetIds { get; set; }

    public IEnumerable<TreeViewItemModel> Controllers { get; set; }
    public IEnumerable<TreeViewItemModel> Widgets { get; set; }
    public IEnumerable<TreeViewItemModel> ChartsTreeView { get; set; }
    public IEnumerable<PostTypeViewModel> Posts { get; set; }
}