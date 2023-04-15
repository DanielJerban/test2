using System.ComponentModel.DataAnnotations;

namespace BPMS.Domain.Common.Dtos;

public class LoginInputDTO
{
    [Required(AllowEmptyStrings = false, ErrorMessage = "وارد کردن نام کاربری اجباری ست")]
    public string Username { get; set; }
    [Required(AllowEmptyStrings = false, ErrorMessage = "وارد کردن رمز عبور اجباری ست")]
    public string Password { get; set; }
    public bool Rememberme { get; set; }
    public bool LoginFailed { get; set; } = false;
    public string RecaptchaType { get; set; }
    public string CaptchaOfflineAnswer { get; set; }

}