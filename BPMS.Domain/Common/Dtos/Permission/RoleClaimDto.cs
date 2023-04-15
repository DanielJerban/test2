namespace BPMS.Domain.Common.Dtos.Permission;

public class RoleClaimDto
{
    public Guid Id { get; set; }
    public Guid RoleId { get; set; }
    public string ClaimType { get; set; }
    public string ClaimValue { get; set; }
}