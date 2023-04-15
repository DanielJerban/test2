namespace BPMS.Domain.Entities;

public class RoleMapPostType
{
    public RoleMapPostType()
    {
        Id = Guid.NewGuid();
    }

    public Guid Id { get; set; }

    public Guid RoleId { get; set; }
    public virtual Role Role { get; set; }

    public Guid PostTypeId { get; set; }
    public virtual LookUp PostType { get; set; }
}