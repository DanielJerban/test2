using System.ComponentModel.DataAnnotations;
using BPMS.Domain.Common.Enums;

namespace BPMS.Domain.Common.CustomFilters;

public class GsmCustomValidation : RequiredAttribute
{
    private string Type { get; }
    public GsmCustomValidation(string type)
    {
        Type = type;
    }
    protected override ValidationResult IsValid(object value, ValidationContext validationContext)
    {
        var TypeEntityObject = validationContext.ObjectType.GetProperty(Type)
            .GetValue(validationContext.ObjectInstance);

        if ((SmsSenderType)TypeEntityObject != SmsSenderType.gsm)
            return ValidationResult.Success;



        if (base.IsValid(value, validationContext) != ValidationResult.Success)
            return new ValidationResult("");


        return ValidationResult.Success;
    }

}