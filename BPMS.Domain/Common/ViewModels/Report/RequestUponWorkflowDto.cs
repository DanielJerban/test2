namespace BPMS.Domain.Common.ViewModels.Report;

public class RequestUponWorkflowDto
{
    public Guid WorkflowId { get; set; }
    public List<RequestWorkFlowDto> RequestWorkFlowDto { get; set; }
}