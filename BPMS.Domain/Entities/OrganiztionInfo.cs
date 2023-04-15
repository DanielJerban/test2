using System.ComponentModel.DataAnnotations;

namespace BPMS.Domain.Entities;

public class OrganiztionInfo
{
    public OrganiztionInfo()
    {
        Id = Guid.NewGuid();
   
    }

    [Key]
    public Guid Id { get; set; }

    public int PhpId { get; set; }

    [Display(Name = "نام کاربری")]
    public Guid StaffId { get; set; }

    [Display(Name = "کد سمت شغلی")]
    public Guid OrganiztionPostId { get; set; }

    [Display(Name = "کد سازمان")]

    public Guid ChartId { get; set; }

    [Display(Name = "اولویت سمت ها")]
    public bool Priority { get; set; }

    [Display(Name = "فعال/غیرفعال")]
    public bool IsActive { get; set; }


        
    public virtual Staff Staff { get; set; }

       
    public virtual Chart Chart { get; set; }

       
    public virtual LookUp OrganiztionPost { get; set; }
}