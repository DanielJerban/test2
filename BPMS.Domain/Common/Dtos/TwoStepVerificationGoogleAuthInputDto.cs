using System.ComponentModel.DataAnnotations;

namespace BPMS.Domain.Common.Dtos;

public class TwoStepVerificationGoogleAuthInputDto
{
    [Required(AllowEmptyStrings = false,ErrorMessage = "وارد کردن کد تایید اجباری می باشد")]
    public string GoogleAuthCode { get; set; }
    public Guid SecretKey { get; set; }
       
    public string CalenderEvents { get; set; }
    public bool RememberMe { get; set; }
    public bool IsRecaptchaEnabled { get; set; }
    public string qrImage { get; set; }
    public string qrKey { get; set; }
    public string RecaptchaMessage { get; set; }
    public string RecaptchaPublicKey { get; set; }
}