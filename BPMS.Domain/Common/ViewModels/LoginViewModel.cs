using System.ComponentModel.DataAnnotations;

namespace BPMS.Domain.Common.ViewModels;

public class LoginViewModel
{
    [Display(Name = "نام کاربری")]
    [Required(ErrorMessage = "{0} را وارد نمایید")]
    public string Username { get; set; }

    [Required(ErrorMessage = "{0} را وارد نمایید")]
    [Display(Name = "کلمه عبور")]
    [DataType(DataType.Password)]
    public string Password { get; set; }

    [Display(Name = "مرا به خاطر بسپار")]
    public bool RememberMe { get; set; }
    public bool IsRecaptchaEnabled { get; set; }
    public string RecaptchaMessage { get; set; }
    public string RecaptchaPublicKey { get; set; }
    public string RecaptchaType { get; set; }
    public string CaptchaOfflineAnswer { get; set; }

    public string CalenderEvents { get; set; }
    public List<string> PermissionList { get; set; }
    public bool LoginFailed { get; set; } = false;
}