using System.ComponentModel.DataAnnotations;
using BPMS.Domain.Common.CustomFilters;
using BPMS.Domain.Common.Enums;

namespace BPMS.Domain.Common.ViewModels;

public class SmsConfigGetListViewModel
{

    public Guid Id { get; set; }
    public Guid IdGsm { get; set; }
    public Guid IdWeb { get; set; }
    public Guid SettingId { get; set; }

    [Display(Name = "نوع ")]
    public SmsSenderType SmsSenderType { get; set; }
    [Display(Name = "نوع ")]
    public SmsSenderType SmsSendType { get; set; }

    public bool IsActiveGsm { get; set; }
    public bool IsActiveWeb { get; set; }

    [Display(Name = "فعال")]
    [WebServiceSmsCustomValidation(nameof(SmsSenderType))]
    public bool IsActive { get; set; }

    [Display(Name = "نام خدمات دهنده")]
    [WebServiceSmsCustomValidation(nameof(SmsSenderType))]
    public string Name { get; set; }

    [Display(Name = "شماره خدمات دهنده")]
    [WebServiceSmsCustomValidation(nameof(SmsSenderType))]
    public string ProviderNumber { get; set; }

    [Display(Name = "نام کاربری")]
    [WebServiceSmsCustomValidation(nameof(SmsSenderType))]
    public string UserName { get; set; }

    [Display(Name = "کلمه عبور")]
    [WebServiceSmsCustomValidation(nameof(SmsSenderType))]
    public string Password { get; set; }

    [Display(Name = "ApiKey")]
    [WebServiceSmsCustomValidation(nameof(SmsSenderType))]
    public string ApiKey { get; set; }

    [Display(Name = "آدرس ارسال")]
    public string Uri { get; set; }

    [Display(Name = "COM پورت")]
    [GsmCustomValidation(nameof(SmsSenderType))]

    public string GsmPort { get; set; }

    [Display(Name = "BaudRate")]
    [GsmCustomValidation(nameof(SmsSenderType))]

    public string GsmPortRate { get; set; }

    public DateTime CreatedDate { get; set; }
}