using BPMS.Domain.Entities;

namespace BPMS.Domain.Common.ViewModels;

public class PackageViewModel
{
    public IEnumerable<WorkFlowForm> Forms { get; set; }
    public Workflow Bpmn { get; set; }
    public LookUpViewModel RequestGroupType { get; set; }
    public LookUpViewModel RequestType { get; set; }
}