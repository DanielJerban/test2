using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace BPMS.Domain.Common.CustomFilters;

public class PhoneNumberValidationAttribute : ValidationAttribute
{

    protected override ValidationResult IsValid(object value, ValidationContext validationContext)
    {
        var phoneNumber = (string)value;

        if (string.IsNullOrEmpty(phoneNumber))
        {
            return new ValidationResult(" تلفن وارد نشده است .");
        }

        if (!Regex.IsMatch(phoneNumber ?? "", @"^0\d{2,3}\d{8}$"))
            return new ValidationResult(". فرمت وارد شده برای تلفن نادرست است");

        return ValidationResult.Success;
    }
}