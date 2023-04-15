using BPMS.Domain.Entities;

namespace BPMS.Domain.Common.ViewModels;

public class FlowStaff
{
    public Guid WorkFlowDetailId { get; set; }
    public Guid RequestId { get; set; }
    public IEnumerable<Guid> StaffIds { get; set; }
    public Guid? CallActivityId { get; set; }
    public List<EsbViewModel> MessageList { get; set; }
    public List<SignalViewModel> SendRemoteId { get; set; }
    public List<WorkflowEsb> WorkflowEsbs { get; set; }
    public string Gateway { get; set; }
    public FlowParam Param { get; set; }
}

public class MainFlowStaff
{
    public List<FlowStaff> FlowStaves { get; set; }
    public List<string> Gateways { get; set; }
    public string Evt;
    public string End;

    public List<WorkflowEsb> WorkflowEsbs { get; set; }
    public bool IsSelectAcceptor { get; set; }
}