namespace BPMS.Domain.Entities;

public class UserClaim
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid UserId { get; set; }
    public User User { get; set; }
    public string? ClaimType { get; set; }
    public string? ClaimValue { get; set; }
}