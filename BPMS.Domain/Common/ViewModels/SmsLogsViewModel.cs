using System.ComponentModel.DataAnnotations;

namespace BPMS.Domain.Common.ViewModels;

public class SmsLogsViewModel
{
    [Display(Name = "شماره خدمات دهنده ")]
    public string SenderNumber { get; set; }

    [Display(Name = "شماره خدمات گیرنده ")]
    public string RecieverNumber { get; set; }
        
    [Display(Name = "تاریخ ")]
    public string SentDate { get; set; }
        
    [Display(Name = "ساعت ")]
    [DisplayFormat(DataFormatString = "{0: ##:##}", ApplyFormatInEditMode = true)]
    public string Time { get; set; }
        
    [Display(Name = "متن پیام ")]
    public string SmsText { get; set; }
        
    [Display(Name = "وضعیت ارسال ")]
    public string SentStatus { get; set; }
        
    [Display(Name = "متن خطا ")]
    public string ErrorMessage { get; set; }
        
    [Display(Name = "نوع ارسال پیام ")]
    public string SmsSendType { get; set; }
}