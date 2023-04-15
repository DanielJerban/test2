using System.ComponentModel.DataAnnotations;

namespace BPMS.Domain.Common.ViewModels;

public class CreateAssignementViewModel
{
    public Guid Id { get; set; }

    public Guid StaffId { get; set; }
    [Display(Name = "نام و نام خانوادگی پرسنل")]
    [Required]
    public string FullName { get; set; }
      
    [Display(Name = "نام کاربری")]

    public string PersonalCode { get; set; }
    [RegularExpression(pattern: @"^$|^([1۱][۰-۹ 0-9]{3}[/\/]([0 ۰][۱-۶ 1-6])[/\/]([0 ۰][۱-۹ 1-9]|[۱۲12][۰-۹ 0-9]|[3۳][01۰۱])|[1۱][۰-۹ 0-9]{3}[/\/]([۰0][۷-۹ 7-9]|[1۱][۰۱۲012])[/\/]([۰0][1-9 ۱-۹]|[12۱۲][0-9 ۰-۹]|(30|۳۰)))$", ErrorMessage = "فرمت {0} درست وارد نشده است")]
    [Display(Name = "تاریخ شروع")]
    [Required(ErrorMessage = "{0} وارد نشده است")]
    public string FromDate { get; set; }
    [RegularExpression(pattern: @"^$|^([1۱][۰-۹ 0-9]{3}[/\/]([0 ۰][۱-۶ 1-6])[/\/]([0 ۰][۱-۹ 1-9]|[۱۲12][۰-۹ 0-9]|[3۳][01۰۱])|[1۱][۰-۹ 0-9]{3}[/\/]([۰0][۷-۹ 7-9]|[1۱][۰۱۲012])[/\/]([۰0][1-9 ۱-۹]|[12۱۲][0-9 ۰-۹]|(30|۳۰)))$", ErrorMessage = "فرمت {0} درست وارد نشده است")]
    [Display(Name = "تاریخ پایان")]
    [Required(ErrorMessage = "{0} وارد نشده است")]

    public string ToDate { get; set; }
    [Display(Name = "فقط درخواست های خودش مشاهده شود")]

    public bool OnlyOwnRequest { get; set; }

    public Guid ConfermAuthorityId { get; set; }
    public Guid UserId { get; set; }

    public StaffDropDownViewModel StaffDropDown { get; set; }
}