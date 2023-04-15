using System.ComponentModel.DataAnnotations;
using BPMS.Domain.Common.Dtos;

namespace BPMS.Domain.Common.ViewModels;

public class CreatePostViewModel
{
    public Guid StaffId { get; set; }

    public string FullName { get; set; }

    [Display(Name = "فعال/غیرفعال")]
    public bool IsActive { get; set; }

    [Display(Name = "پست اصلی")]
    public bool IsMain { get; set; }

    [Display(Name = "نوع پست سازمانی")]
    [Required(ErrorMessage = "{0} وارد نشده است")]
    public Guid PostTypeId { get; set; }

    public string PostTypeName { get; set; }

    [Display(Name = "عنوان پست سازمانی ")]
    [Required(ErrorMessage = "{0} وارد نشده است")]
    public Guid OrganiztionPostId { get; set; }

    public Guid MainPostId { get; set; }

    [Display(Name = "چارت")]
    [Required(ErrorMessage = "{0} انتخاب نشده است")]
    public string ChartId { get; set; }

    public string PostTitleName { get; set; }

    public bool OnceCheckedBefore { get; set; } = false;

    public List<SelectListItem> PostTypeListItems { get; set; }
    public List<SelectListItem> PostTitleListItems { get; set; }
}