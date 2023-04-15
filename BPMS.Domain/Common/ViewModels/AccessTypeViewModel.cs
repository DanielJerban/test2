using System.ComponentModel.DataAnnotations;

namespace BPMS.Domain.Common.ViewModels;

public class AccessTypeViewModel
{
    public Guid Id { get; set; }
    public Guid FormClassificationAccessId { get; set; }
    [Display(Name = "مصوب")]
    public bool IsApproved { get; set; } = true;
    [Display(Name = "تحت بررسی")]
    public bool IsInProcess { get; set; }
    [Display(Name = "منسوخ")]
    public bool IsExpired { get; set; }
    [Display(Name = "ایجاد نسخه جدید مدرک")]
    public bool CanCreate { get; set; }
    [Display(Name = "ویرایش")]
    public bool CanEdit { get; set; }
    [Display(Name = "حذف")]
    public bool CanRemove { get; set; }
}