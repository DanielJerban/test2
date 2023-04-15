using System.ComponentModel.DataAnnotations;

namespace BPMS.Domain.Common.ViewModels;

public class PropertySubformViewModel
{
    [Display(Name = "شناسه")]
    public string Property { get; set; }

    [Display(Name = "نام فیلد")]
    public string Label { get; set; }
        
    [Display(Name = "نام زیرفرم")]
    public string SubformName { get; set; }

    [Display(Name = "قابلیت ویرایش")]
    public bool IsChecked { get; set; }
}