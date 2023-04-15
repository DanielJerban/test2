using BPMS.Domain.Common.Enums;
using System.ComponentModel.DataAnnotations;

namespace BPMS.Domain.Common.CustomFilters;

public class WebServiceSmsCustomValidation : RequiredAttribute
{
    private string Type { get; }
    public WebServiceSmsCustomValidation(string type)
    {
        Type = type;
    }
    protected override ValidationResult IsValid(object value, ValidationContext validationContext)
    {
        var TypeEntityObject = validationContext.ObjectType.GetProperty(Type)
            .GetValue(validationContext.ObjectInstance);

        if ((SmsSenderType)TypeEntityObject != SmsSenderType.webservice)
            return ValidationResult.Success;



        if (base.IsValid(value, validationContext) != ValidationResult.Success)
            return new ValidationResult("");


        return ValidationResult.Success;
    }

}