using BPMS.Domain.Entities;

namespace BPMS.Domain.Common.ViewModels;

public class GenerateEntityViewModel
{
    public GenerateEntityViewModel()
    {
        WorkFlowBoundary = new List<WorkFlowBoundary>();
        WorkFlowDetails = new List<WorkFlowDetail>();
        WorkFlowNextSteps = new List<WorkFlowNextStep>();
        WorkFlowEsb = new List<WorkflowEsb>();
        TimerStartEvent = new StartTimerEvent();
    }
    public List<WorkFlowDetail> WorkFlowDetails { get; set; }
    public List<WorkFlowNextStep> WorkFlowNextSteps { get; set; }
    public List<WorkFlowBoundary> WorkFlowBoundary { get; set; }
    public List<WorkflowEsb> WorkFlowEsb { get; set; }
    public StartTimerEvent TimerStartEvent { get; set; }
}