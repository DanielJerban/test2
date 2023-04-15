using System.ComponentModel.DataAnnotations;

namespace BPMS.Domain.Common.Dtos;

public class TwoStepVerificationEmailInputDto
{
    [Required(AllowEmptyStrings = false,ErrorMessage = "وارد کردن کد تایید اجباری می باشد")]
    public string EmailCode { get; set; }
    public Guid SecretKey { get; set; }
    public string Email { get; set; }
    public string CalenderEvents { get; set; }
    public string RecaptchaMessage { get; set; }
    public bool RememberMe { get; set; }
    public bool IsRecaptchaEnabled { get; set; }
    public string RecaptchaPublicKey { get; set; }
    public bool PostBack { get; set; }
    public int Counter { get; set; }
}