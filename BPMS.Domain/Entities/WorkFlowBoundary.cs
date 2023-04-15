namespace BPMS.Domain.Entities;

public class WorkFlowBoundary
{
    public WorkFlowBoundary()
    {
        Id = Guid.NewGuid();
    }
    public Guid Id { get; set; }
    public Guid WorkflowDetailId { get; set; }
    public string? BoundaryId { get; set; }
    public string? Info { get; set; }


    public WorkFlowDetail WorkFlowDetail { get; set; }
}