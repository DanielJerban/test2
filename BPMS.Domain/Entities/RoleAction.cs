using System.ComponentModel.DataAnnotations;

namespace BPMS.Domain.Entities;

public class RoleAction
{
    public RoleAction()
    {
        Id = Guid.NewGuid();
    }

    public Guid Id { get; set; }

    [MaxLength(50)]
    public string? Controller { get; set; }

    [MaxLength(50)]
    public string? Action { get; set; }

    public Guid RoleId { get; set; }
    public virtual Role Role { get; set; }
}