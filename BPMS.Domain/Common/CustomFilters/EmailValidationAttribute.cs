using System.ComponentModel.DataAnnotations;

namespace BPMS.Domain.Common.CustomFilters;

public class EmailValidationAttribute : RegularExpressionAttribute
{
    public EmailValidationAttribute() : base(@"^([A-Za-z0-9_\-.+])+@([A-Za-z0-9_\-.])+\.([A-Za-z]{2,})$")
    {

    }
}