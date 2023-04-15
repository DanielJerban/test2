namespace BPMS.Domain.Common.ViewModels;

public class FlowEventViewModel
{
    public Guid Id { get; set; }
    public Guid WorkFlowEsbId { get; set; }
    public Guid FlowId { get; set; }
    public string Value { get; set; }
    public bool IsActive { get; set; }
}