using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace BPMS.Domain.Common.CustomFilters;

public class MobileValidationAttribute : ValidationAttribute
{
    protected override ValidationResult IsValid(object value, ValidationContext validationContext)
    {
        var mobile = (string)value;

        if (string.IsNullOrEmpty(mobile))
        {
            return new ValidationResult(" تلفن  همراه وارد نشده است .");
        }

        if (!Regex.IsMatch(mobile ?? "", @"^09(0[0-9]|1[0-9]|3[1-9]|2[1-9])-?[0-9]{3}-?[0-9]{4}$"))
            return new ValidationResult(". فرمت وارد شده برای تلفن همراه نادرست است");

        return ValidationResult.Success;
    }
}