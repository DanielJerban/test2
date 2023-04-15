using System.ComponentModel.DataAnnotations;

namespace BPMS.Domain.Common.Enums;

public enum ScheduleType
{
    [Display(Name = "محاسبه میانگین انجام درخواست")]
    CalculateAverageProcessing = 0,

    [Display(Name = "همگام سازی LDAP")]
    ActivateLdapSyncer = 1,

    [Display(Name = "رخداد تایمر مرزی مستمر")]
    ActivateNonInterruptBoundary = 2,

    [Display(Name = "رخداد تایمر مرزی قطع کننده")]
    ActivateInterruptBoundary = 3,

    [Display(Name = "رخداد تایمر میانی")]
    ActivateIntermediateTimerNotation = 4,

    [Display(Name = "محاسبه تاخیر تمامی درخواست ها")]
    CalculateAllRequestsDelay = 5,

    [Display(Name = "رخداد تایمر شروع")]
    RunTimerStartEvent = 6
}