using System.ComponentModel.DataAnnotations;

namespace BPMS.Domain.Common.ViewModels;

public class ElmahErrorLogsViewModel
{
    [Display(Name ="نام نرم افزار")]
    public string Application { get; set; }

    [Display(Name ="سرور")]
    public string Host { get; set; }

    [Display(Name ="نوع")]
    public string Type { get; set; }

    [Display(Name ="منبع")]
    public string Source { get; set; }

    [Display(Name ="پیام")]
    public string Message { get; set; }

    [Display(Name ="کاربر")]
    public string User { get; set; }

    [Display(Name ="کد وضعیت")]
    public string StatusCode { get; set; }

    [Display(Name ="تاریخ")]
    public string Date { get; set; }

    [Display(Name ="زمان")]
    public string Time { get; set; }

    [Display(Name ="شناسه یکتا")]
    public string Sequence { get; set; }

    [Display(Name ="جزئیات خطا")]
    public string AllXml { get; set; }
}