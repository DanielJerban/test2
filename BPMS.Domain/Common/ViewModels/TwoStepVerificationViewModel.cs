using BPMS.Domain.Common.CustomFilters;

namespace BPMS.Domain.Common.ViewModels;

public class TwoStepVerificationViewModel
{
    public bool TwoStepVerification { get; set; }
    public bool TwoStepVerificationByEmail { get; set; }
    public bool EnableRecaptcha { get; set; }
    public bool TwoStepVerificationByGoogleAuthenticator { get; set; }

    [TwostepVerificationBySmsValidation(nameof(TwoStepVerification))]
    [MobileValidation(ErrorMessage = "فرمت موبایل نادرست است")]
    public string PhoneNumber { get; set; }

    [TwostepVerificationByEmailValidation(nameof(TwoStepVerificationByEmail))]
    [TwoStepVerificationByGoogleAuthenticatorValidation(nameof(TwoStepVerificationByGoogleAuthenticator))]
    [EmailValidationAttribute(ErrorMessage = "فرمت ایمیل نادرست است")]
    public string Email { get; set; }
}