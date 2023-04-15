namespace BPMS.Domain.Common.Dtos.Permission;

public class ControllerPolicyDTO
{
    public string Name { get; set; }
    public string Policy { get; set; }
    public IEnumerable<ActionPolicyDTO> ActionPolicies { get; set; }
}