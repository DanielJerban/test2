using BPMS.Domain.Common.Enums;
using System.ComponentModel.DataAnnotations;
using BPMS.Domain.Common.CustomFilters;

namespace BPMS.Domain.Common.ViewModels;

public class SmsConfigViewModel
{
    public Guid Id { get; set; }
    public Guid IdGsm { get; set; }
    public Guid IdWeb { get; set; }

    [Display(Name = "نوع ")]
    public SmsSenderType SmsSenderType { get; set; }

    public bool IsActiveGsm { get; set; }
    public bool IsActiveWeb { get; set; }

    [Display(Name = "فعال")]
    [WebServiceSmsCustomValidation(nameof(SmsSenderType))]
    public bool IsActive { get; set; }

    [Display(Name = "نام خدمات دهنده :")]
    [Required(ErrorMessage = "پر کردن فیلد نام خدمات دهنده اجباری است")]
    public string Name { get; set; }

    [Display(Name = "شماره خدمات دهنده :")]
    [Required(ErrorMessage = "پر کردن فیلد شماره خدمات دهنده اجباری است")]
    public string ProviderNumber { get; set; }

    [Display(Name = "نام کاربری :")]
    [Required(ErrorMessage = "پر کردن فیلد نام کاربری اجباری است")]
    public string UserName { get; set; }

    [Display(Name = "کلمه عبور :")]
    [Required(ErrorMessage = "پر کردن فیلد کلمه عبور اجباری است")]
    public string Password { get; set; }

    [Display(Name = "ApiKey : ")]
    public string ApiKey { get; set; }

    [Display(Name = "آدرس ارسال :")]
    public string Uri { get; set; }

    [Display(Name = ": COM پورت")]
    [Required(ErrorMessage = "پر کردن فیلد COM  اجباری است")]
    public string GsmPort { get; set; }

    [Display(Name = ": BaudRate")]
    [Required(ErrorMessage = "پر کردن فیلدBaudRate اجباری است")]
    public string GsmPortRate { get; set; }

}