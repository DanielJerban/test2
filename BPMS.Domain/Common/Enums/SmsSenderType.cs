using System.ComponentModel.DataAnnotations;

namespace BPMS.Domain.Common.Enums;

public enum SmsSenderType
{
    [Display(Name = "مودم جی اس ام")]
    gsm = 1,
    [Display(Name = "وب سرویس")]
    webservice = 2
}