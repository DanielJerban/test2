using System.ComponentModel.DataAnnotations;
using BPMS.Domain.Common.Dtos;

namespace BPMS.Domain.Common.ViewModels;

public class ChartViewModel
{
    public Guid Id { get; set; }
    [Display(Name = "عنوان")]

    public string Title { get; set; }
    [Display(Name = "سطح چارت")]
    public Guid ChartLevelId { get; set; }

    [Display(Name = "زیر مجموعه")]
    public Guid? ParentId { get; set; }

    [Display(Name = "فعال")]
    public bool IsActive { get; set; }
    //این فیلد برای استفاده در گرید قسمت سطح دسترسی استفاده شده است.
    public string ParentTitle { get; set; }

    public List<SelectListItem> ChartLevelList { get; set; }
}