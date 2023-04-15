namespace BPMS.Domain.Entities;

public class WorkflowDetailPattern
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string? Name { get; set; }
    public virtual ICollection<WorkflowDetailPatternItem> WorkflowPatternItems { get; set; }
    public virtual ICollection<WorkFlowDetail> WorkFlowDetails { get; set; }
}