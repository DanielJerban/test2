using BPMS.Domain.Entities;

namespace BPMS.Domain.Common.ViewModels.Report;

public class RequestWorkFlowDto
{
    public Guid RequestTypeId { get; set; }
    public Guid RequestId { get; set; }
    public Guid WorkflowId { get; set; }
    public Request Request { get; set; }
    public Workflow Workflow { get; set; }
    public string WorkFlowTitle { get; set; }
}