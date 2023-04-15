using System.ComponentModel.DataAnnotations;

namespace BPMS.Infrastructure.Filters;

public class TwoStepVerificationByGoogleAuthenticatorValidation : RequiredAttribute
{
    private string EmailVerificationEntityName { get; }
    public TwoStepVerificationByGoogleAuthenticatorValidation(string emailVerificationEntityName)
    {
        EmailVerificationEntityName = emailVerificationEntityName;
    }

    protected override ValidationResult IsValid(object value, ValidationContext validationContext)
    {
        var emailVerificationEntityObject = validationContext.ObjectType.GetProperty(EmailVerificationEntityName)
            .GetValue(validationContext.ObjectInstance);
        if ((bool)emailVerificationEntityObject && (base.IsValid(value, validationContext) != ValidationResult.Success))
        {
            return new ValidationResult(" آدرس ایمیل وارد نشده است .");
        }
        return ValidationResult.Success;
    }
}