namespace BPMS.Domain.Entities;

public class RoleAccess
{
    public RoleAccess()
    {
        Id = Guid.NewGuid();
    }

    public Guid Id { get; set; }


    public Guid RoleId { get; set; }
    public virtual Role Role { get; set; }

    public Guid UserId { get; set; }
    public virtual User User { get; set; }
}