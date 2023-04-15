using System.ComponentModel.DataAnnotations;

namespace BPMS.Domain.Entities;

public class Role
{
    public Role()
    {
        Id = Guid.NewGuid();
        RoleAccesses = new HashSet<RoleAccess>();
        RoleMapCharts = new HashSet<RoleMapChart>();
        RoleMapPostTypes = new HashSet<RoleMapPostType>();
        RoleMapPostTitles = new HashSet<RoleMapPostTitle>();
    }
    public Guid Id { get; set; }

    [Display(Name = "نام نقش")]
    [Required(ErrorMessage = "{0} را وارد نمایید.")]
    public string Name { get; set; }

    [Display(Name = "توضیحات")]
    public string? Dsr { get; set; }


    public ICollection<RoleClaim> RoleClaims { get; set; }
    public virtual ICollection<RoleAccess> RoleAccesses { get; set; }
    public virtual ICollection<RoleMapChart> RoleMapCharts { get; set; }
    public virtual ICollection<RoleMapPostType> RoleMapPostTypes { get; set; }
    public virtual ICollection<RoleMapPostTitle> RoleMapPostTitles { get; set; }
}