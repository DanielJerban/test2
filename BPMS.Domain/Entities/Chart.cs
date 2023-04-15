using System.ComponentModel.DataAnnotations;

namespace BPMS.Domain.Entities;

public class Chart
{
    public Chart()
    {
        Id = Guid.NewGuid();
        OrganiztionInfos = new HashSet<OrganiztionInfo>();
        RoleMapCharts = new HashSet<RoleMapChart>();

    }
        
    [Key]
    [Display(Name = "شناسه")]
    public Guid Id { get; set; }

    public int PhpId { get; set; }

    [Display(Name = "عنوان")]
    public string? Title { get; set; }
        
    [Display(Name = "سطح چارت")]
    public Guid ChartLevelId { get; set; }
        
    [Display(Name = "زیر مجموعه")]
    public Guid? ParentId { get; set; }
        
    [Display(Name = "فعال/غیرفعال")]
    public bool IsActive { get; set; }


    //Navigation Property
       
    public virtual Chart ChartParent { get; set; }
    public virtual ICollection<Chart> ChartChild { get; set; }

    public virtual LookUp ChartLevel { get; set; }
    public virtual ICollection<OrganiztionInfo> OrganiztionInfos { get; set; }
    public virtual ICollection<RoleMapChart> RoleMapCharts { get; set; }

}