using System.ComponentModel.DataAnnotations;
using BPMS.Domain.Common.Dtos;

namespace BPMS.Domain.Common.ViewModels;

public class HolydayViewModel
{
    public Guid Id { get; set; }
    [Display(Name = "روز تعطیل")]
    [Required(ErrorMessage = "{0} وارد نشده است")]
    public string Date { get; set; }
    [Display(Name = "توضیحات")]
    public string Dsr { get; set; }
    public Guid HolydayTypeId { get; set; }
    public IEnumerable<SelectListItem> HolydayTypeSelectItem { get; set; }
    [Display(Name = "نوع روز تعطیل")]
    public string HolydayType { get; set; }

    [Display(Name = "ساعت شروع شنبه تا چهارشنبه")]
    [Required(ErrorMessage = "{0} وارد نشده است")]
    [RegularExpression(@"^(?:[01][0-9]|2[0-3]):[0-5][0-9]$", ErrorMessage = "فرمت وارد شده نادرست است")]
    public string StartTime { get; set; }

    [Display(Name = "ساعت شروع پنجشنبه")]
    [Required(ErrorMessage = "{0} وارد نشده است")]
    [RegularExpression(@"^(?:[01][0-9]|2[0-3]):[0-5][0-9]$", ErrorMessage = "فرمت وارد شده نادرست است")]
    public string StartTimeThr { get; set; }

    [Display(Name = "ساعت پایان شنبه تا چهارشنبه")]
    [Required(ErrorMessage = "{0} وارد نشده است")]
    [RegularExpression(@"^(?:[01][0-9]|2[0-3]):[0-5][0-9]$", ErrorMessage = "فرمت وارد شده نادرست است")]
    public string EndTime { get; set; }

    [Display(Name = "ساعت پایان پنجشنبه")]
    [Required(ErrorMessage = "{0} وارد نشده است")]
    [RegularExpression(@"^(?:[01][0-9]|2[0-3]):[0-5][0-9]$", ErrorMessage = "فرمت وارد شده نادرست است")]
    public string EndTimeThr { get; set; }

}