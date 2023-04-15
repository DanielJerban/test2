namespace BPMS.Domain.Entities;

public class RoleClaim
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid RoleId { get; set; }
    public Role Role { get; set; }
    public string? ClaimType { get; set; }
    public string? ClaimValue { get; set; }
}