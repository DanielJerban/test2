using System.ComponentModel.DataAnnotations;

namespace BPMS.Infrastructure.Filters;

public class RecaptchaRequirementValidation : RequiredAttribute
{
    private string IsRecaptchaEnableEntityName { get; }
    public RecaptchaRequirementValidation(string isRecaptchaEnableEntityName)
    {
        IsRecaptchaEnableEntityName = isRecaptchaEnableEntityName;
    }

    protected override ValidationResult IsValid(object value, ValidationContext validationContext)
    {
        var IsRecaptchaEnableEntityObject = validationContext.ObjectType.GetProperty(IsRecaptchaEnableEntityName)
            .GetValue(validationContext.ObjectInstance);
        if ((bool)IsRecaptchaEnableEntityObject && (base.IsValid(value, validationContext) != ValidationResult.Success))
        {
            return new ValidationResult(" کلید ها را وارد نمایید");
        }
        return ValidationResult.Success;
    }
}