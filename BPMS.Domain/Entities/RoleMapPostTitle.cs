namespace BPMS.Domain.Entities;

public class RoleMapPostTitle
{
    public RoleMapPostTitle()
    {
        Id = Guid.NewGuid();
    }

    public Guid Id { get; set; }

    public Guid RoleId { get; set; }
    public virtual Role Role { get; set; }

    public Guid PostTitleId { get; set; }
    public virtual LookUp PostTitle { get; set; }
}