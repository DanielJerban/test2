using System.ComponentModel.DataAnnotations;

namespace BPMS.Domain.Common.ViewModels;

public class EmailLogsViewModel
{
    [Display(Name = "ایمیل خدمات دهنده ")]
    public string SenderEmail { get; set; }
    [Display(Name = "ایمیل خدمات گیرنده ")]
    public string RecieverEmail { get; set; }
    [Display(Name = "تاریخ ")]
    public string SentDate { get; set; }
    [Display(Name = "ساعت ")]
    [DisplayFormat(DataFormatString = "{0: ##:##}", ApplyFormatInEditMode = true)]
    public string Time { get; set; }
    [Display(Name = "متن ایمیل ")]
    public string EmailText { get; set; }
    [Display(Name = "وضعیت ارسال ")]
    public string SentStatus { get; set; }
    [Display(Name = "متن خطا ")]
    public string ErrorMessage { get; set; }
}