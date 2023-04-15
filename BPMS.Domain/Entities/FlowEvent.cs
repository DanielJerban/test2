namespace BPMS.Domain.Entities;

public class FlowEvent
{
    public FlowEvent()
    {
        Id = Guid.NewGuid();

    }
    public Guid Id { get; set; }
    public Guid WorkFlowEsbId { get; set; }
    public Guid FlowId { get; set; }
    public string? Value { get; set; }
    public bool IsActive { get; set; }
    public int Order { get; set; }
    public string? GatewayEventBase { get; set; }

    public virtual Flow Flow { get; set; }
    public virtual WorkflowEsb WorkflowEsb { get; set; }
}