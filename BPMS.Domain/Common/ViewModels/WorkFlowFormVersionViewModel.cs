using System.ComponentModel.DataAnnotations;

namespace BPMS.Domain.Common.ViewModels;

public class WorkFlowFormVersionViewModel
{
    [Display(Name = "نسخه اصلی")]
    [Required(ErrorMessage = "{0} را وارد نمایید.")]
    [Range(0, 100, ErrorMessage = "نسخه اصلی باید بین 0 تا 100 باشد")]
    public int OrginalVersion { get; set; }

    [Display(Name = "نسخه فرعی")]
    [Required(ErrorMessage = "{0} را وارد نمایید.")]
    [Range(0, 9, ErrorMessage = "نسخه فرعی باید بین 0 تا 9 باشد")]
    public int SecondaryVersion { get; set; }

    [Display(Name = "نسخه")]
    public string Version => $"{OrginalVersion},{SecondaryVersion}";
       

}