using System.ComponentModel.DataAnnotations;

namespace BPMS.Domain.Common.ViewModels;

public class ExceptionsViewModel
{
    public Guid Id { get; set; }

    [Display(Name = "شماره")]
    public long Number { get; set; }

    [Display(Name = "تاریخ رویداد ")]
    public DateTime CreateDate { get; set; }
        
    [Display(Name = "متن خطا")]
    public string? Content { get; set; }
        
    [Display(Name = "نام کاربری")]
    public string? UserName { get; set; }
        
    [Display(Name = "رویت شده؟")]
    public bool IsRead { get; set; }
        
    [Display(Name = "IP")]
    public string? IpAddress { get; set; }
}