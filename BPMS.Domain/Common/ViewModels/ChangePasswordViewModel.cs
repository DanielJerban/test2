using System.ComponentModel.DataAnnotations;

namespace BPMS.Domain.Common.ViewModels;

public class ChangePasswordViewModel
{
    [Display(Name = "کلمه عبور قبلی ")]
    [Required(ErrorMessage = "{0} را وارد نمایید")]
    public string OldPassword { get; set; }

    [Required(ErrorMessage = "{0} را وارد نمایید")]
    [Display(Name = "کلمه عبور جدید")]
    [RegularExpression(@"^(?=.*[A-Za-z])(?=.*[0-9]).{6,}$", ErrorMessage = "کلمه عبور باید حداقل 6 کارکتر و شامل اعداد و حروف باشد")]
    public string NewPassword { get; set; }

    [Required(ErrorMessage = "{0} را وارد نمایید")]
    [RegularExpression(@"^(?=.*[A-Za-z])(?=.*[0-9]).{6,}$", ErrorMessage = "تکرار کلمه عبور باید حداقل 6 کارکتر و شامل اعداد و حروف باشد")]
    [Display(Name = "تکرار کلمه عبور جدید ")]
    [Compare(nameof(NewPassword), ErrorMessage = "کلمه عبور و تکرار آن برابر نمی باشد")]
        
    public string RepeatNewPassword { get; set; }
    public string RecaptchaMessage { get; set; }
    public bool IsRecaptchaEnabled { get; set; }
    public string RecaptchaPublicKey { get; set; }
}