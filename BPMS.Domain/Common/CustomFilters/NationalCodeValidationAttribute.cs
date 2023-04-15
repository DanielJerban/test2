using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace BPMS.Domain.Common.CustomFilters;

public class NationalCodeValidationAttribute : ValidationAttribute
{
    protected override ValidationResult IsValid(object value, ValidationContext validationContext)
    {
        var nationalCode = (string)value;
        return ValidateNationalCode(nationalCode);
    }

    private ValidationResult ValidateNationalCode(string nationalCode)
    {
        if (string.IsNullOrEmpty(nationalCode))
        {
            return new ValidationResult(" کد ملی وارد نشده است .");
        }

        if (!Regex.IsMatch(nationalCode, @"^\d{10}$"))
            return new ValidationResult(" کد ملی باید تشکیل شده از 10 عدد و فاقد حرف باشد .");


        int sum = 0;
        var check = Convert.ToInt32(nationalCode.Substring(9, 1));

        sum = Enumerable.Range(0, 9)
            .Select(x => Convert.ToInt32(nationalCode.Substring(x, 1)) * (10 - x))
            .Sum() % 11;

        if (!((sum < 2 && check == sum) || (sum >= 2 && check + sum == 11)))
            return new ValidationResult(" کد ملی مورد نظر صحیح نمی باشد .");

        if (HasTenSameCharecters(nationalCode, 10))
            return new ValidationResult(" کد ملی مورد نظر صحیح نمی باشد .");

        return ValidationResult.Success;
    }

    private bool HasTenSameCharecters(string code, int sequenceLength)
    {
        return Regex.IsMatch(code, "^(.)\\1{" + (sequenceLength - 1) + "}$");

    }
}