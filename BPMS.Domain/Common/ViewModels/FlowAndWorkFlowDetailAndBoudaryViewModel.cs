using BPMS.Domain.Entities;

namespace BPMS.Domain.Common.ViewModels;

public class FlowAndWorkFlowDetailAndBoudaryViewModel
{
    public Flow Flow { get; set; }
    public WorkFlowDetail WorkFlowDetail { get; set; }
    public WorkFlowBoundary WorkFlowBoundary { get; set; }
}