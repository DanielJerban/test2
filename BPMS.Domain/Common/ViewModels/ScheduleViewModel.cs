using System.ComponentModel.DataAnnotations;
using BPMS.Domain.Common.Dtos;

namespace BPMS.Domain.Common.ViewModels;

public class ScheduleViewModel
{
    public Guid? ScheduleId { get; set; }
    [Display(Name = "عنوان")]
    [Required(ErrorMessage = "{0} وارد نشده است")]
    public string TitleShchedules { get; set; }

    [Display(Name = "نوع کار")]
    public string TaskType { get; set; }
    [Display(Name = "نوع کار")]
    [Required(ErrorMessage = "{0} وارد نشده است")]
    public Guid TaskTypeId { get; set; }
    [Display(Name = "فعال")]
    public bool IsActive { get; set; }
    [Display(Name = "تاریخ شروع")]
    [RegularExpression(pattern: @"^$|^([1۱][۰-۹ 0-9]{3}[/\/]([0 ۰][۱-۶ 1-6])[/\/]([0 ۰][۱-۹ 1-9]|[۱۲12][۰-۹ 0-9]|[3۳][01۰۱])|[1۱][۰-۹ 0-9]{3}[/\/]([۰0][۷-۹ 7-9]|[1۱][۰۱۲012])[/\/]([۰0][1-9 ۱-۹]|[12۱۲][0-9 ۰-۹]|(30|۳۰)))$", ErrorMessage = "فرمت {0} درست وارد نشده است")]
    [Required(ErrorMessage = "{0} وارد نشده است")]
    public string StartDate { get; set; }
    [Display(Name = "تاریخ شروع")]
    public int StartDateInt { get; set; }
    [Display(Name = "تاریخ پایان")]
    [Required(ErrorMessage = "{0} وارد نشده است")]
    [RegularExpression(pattern: @"^$|^([1۱][۰-۹ 0-9]{3}[/\/]([0 ۰][۱-۶ 1-6])[/\/]([0 ۰][۱-۹ 1-9]|[۱۲12][۰-۹ 0-9]|[3۳][01۰۱])|[1۱][۰-۹ 0-9]{3}[/\/]([۰0][۷-۹ 7-9]|[1۱][۰۱۲012])[/\/]([۰0][1-9 ۱-۹]|[12۱۲][0-9 ۰-۹]|(30|۳۰)))$", ErrorMessage = "فرمت {0} درست وارد نشده است")]
    public string EndDate { get; set; }
    [Display(Name = "تاریخ پایان")]
    public int EndDateInt { get; set; }
    [Display(Name = "ساعت اجرا")]
    [RegularExpression(pattern: @"^(0[0-9]|1[0-9]|2[0-3]):[0-5][0-9]$", ErrorMessage = "فرمت {0} درست وارد نشده است")]
    [Required(ErrorMessage = "{0} وارد نشده است")]
    public string RunTime { get; set; }
    [Display(Name = "به صورت روزانه اجرا شود")]
    public bool IsDaily { get; set; }
    [Display(Name = "شنبه")]
    public bool SaturDay { get; set; }
    [Display(Name = "یکشنبه")]
    public bool SunDay { get; set; }
    [Display(Name = "دوشنبه")]
    public bool MonDay { get; set; }
    [Display(Name = "سه شنبه")]
    public bool TuesDay { get; set; }
    [Display(Name = "چهارشنبه")]
    public bool WednesDay { get; set; }
    [Display(Name = "پنجشنبه")]
    public bool ThursDay { get; set; }
    [Display(Name = "جمعه")]
    public bool Friday { get; set; }
    [Display(Name = "اجرای مجدد درصورت عدم اجرا در ساعت معین شده ")]
    public bool IsRunExpireTrigger { get; set; }
    [Display(Name = "فاصله روزانه")]
    [Required(ErrorMessage = "{0} وارد نشده است")]
    [Range(0, 255, ErrorMessage = "عدد وارد شده باید بین 0 تا 255 باشد")]
    public byte DailyInterval { get; set; }
    [Display(Name = "تاریخ ثبت")]
    public int RegisterDate { get; set; }

    public string Code { get; set; }

    public IEnumerable<SelectListItem> ListItems { get; set; }
}