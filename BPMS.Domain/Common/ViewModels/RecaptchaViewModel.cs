using System.ComponentModel.DataAnnotations;

namespace BPMS.Domain.Common.ViewModels;

public class RecaptchaViewModel
{
    [Display(Name = "فعال کردن ریکپچا")]
    public bool IsRecaptchaEnable { get; set; }

    [Display(Name = "SecretKey ریکپچا")]
    [Required(ErrorMessage = "این فیلد الزامی میباشد")]
    public string RecaptchaSecretKey { get; set; }

    [Display(Name = "PublicKey ریکپچا")]
    [Required(ErrorMessage = "این فیلد الزامی میباشد")]
    public string RecaptchaPublicKey { get; set; }

    public string RecaptchaMessage { get; set; }
    public string Response { get; set; }
    public string RecaptchaType { get; set; }
    public string LastEnabledRecaptchaType { get; set; }
}