using System.ComponentModel.DataAnnotations;
namespace BPMS.Domain.Common.Enums;

public enum ContentTypeEnum
{
    [Display(Name = "application/json")]
    Json,
    [Display(Name = "form-data")]
    FormData
}