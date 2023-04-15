namespace BPMS.Domain.Entities;

public class WorkflowDetailPatternItem
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public int Index { get; set; }

    public Guid LookupOrganizationPostId { get; set; }
    public LookUp LookUpOrganizationPost { get; set; }

    public Guid WorkflowDetailPatternId { get; set; }
    public virtual WorkflowDetailPattern WorkflowDetailPattern { get; set; }
}