using System.ComponentModel.DataAnnotations;

namespace BPMS.Domain.Entities;

public class WorkFlowNextStep
{
    public WorkFlowNextStep()
    {
        Id = Guid.NewGuid();
        WorkflowEsbs = new HashSet<WorkflowEsb>();
    }

    [Key]
    public Guid Id { get; set; }

    public Guid FromWfdId { get; set; }

    public Guid ToWfdId { get; set; }

    public string? Path { get; set; }
    public string? BoundaryName { get; set; }

    public string? Exp { get; set; }
    public string? Esb { get; set; }
    public string? Gateway { get; set; }
    public string? FlowLine { get; set; }

    public string? Method { get; set; }
    public string? Evt { get; set; }
    //Navigation Property

    public virtual WorkFlowDetail NextStepFromWfd { get; set; }
    public virtual WorkFlowDetail NextStepToWfd { get; set; }

    public virtual ICollection<WorkflowEsb> WorkflowEsbs { get; set; }
}