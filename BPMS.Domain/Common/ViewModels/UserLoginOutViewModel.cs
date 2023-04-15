using System.ComponentModel.DataAnnotations;

namespace BPMS.Domain.Common.ViewModels;

public class UserLoginOutViewModel
{
    [Display(Name = "نام و نام خانوادگی")]
    public string FullName { get; set; }

    [Display(Name = "نام کاربری")]
    public string UserName { get; set; }

    [Display(Name = "نوع ")]
    public string LoginOut { get; set; }

    [Display(Name = "تاریخ")]
    public string Date { get; set; }

    [Display(Name = "ساعت")]
    public string Time { get; set; }

    [Display(Name = "IP کاربر")]
    public string Ip { get; set; }

    [Display(Name = "عنوان سیستم")]
    public string MachineName { get; set; }

    [Display(Name = " عنوان مرورگر")]
    public string BrowserName { get; set; }
}