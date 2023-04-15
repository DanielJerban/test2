using System.ComponentModel.DataAnnotations;

namespace BPMS.Infrastructure.Filters;

public class TwostepVerificationBySmsValidation : RequiredAttribute
{
    private string SmsVerificationEntityName { get; }
    public TwostepVerificationBySmsValidation(string smsVerificationEntityName)
    {
        SmsVerificationEntityName = smsVerificationEntityName;
    }

    protected override ValidationResult IsValid(object value, ValidationContext validationContext)
    {
        var SmsVerificationEntityObject = validationContext.ObjectType.GetProperty(SmsVerificationEntityName)
            .GetValue(validationContext.ObjectInstance);
        if ((bool)SmsVerificationEntityObject && (base.IsValid(value, validationContext) != ValidationResult.Success))
        {
            return new ValidationResult(" شماره موبایل وارد نشده است .");
        }
        return ValidationResult.Success;
    }
}