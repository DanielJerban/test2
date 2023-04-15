using System.ComponentModel.DataAnnotations;

namespace BPMS.Domain.Entities;

public class WorkflowEsb
{
    public WorkflowEsb()
    {
        Id = Guid.NewGuid();
        FlowEvents = new HashSet<FlowEvent>();
    }
    [Key]
    public Guid Id { get; set; }
    public string? Info { get; set; }
    public string? EventId { get; set; }
    public Guid WorkflowNextStepId { get; set; }
    //Navigation Property 
    public virtual WorkFlowNextStep WorkFlowNextStep { get; set; }
    public virtual ICollection<FlowEvent> FlowEvents { get; set; }

}