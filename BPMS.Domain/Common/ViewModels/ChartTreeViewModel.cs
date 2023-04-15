using System.ComponentModel.DataAnnotations;

namespace BPMS.Domain.Common.ViewModels;

public class ChartTreeViewModel
{
    [Display(Name = "نام کاربری")]
    public Guid Id { get; set; }

    [Display(Name = "نام")]
    public string Name { get; set; }

    [Display(Name = "نام خانوادگی")]
    public string Family { get; set; }

    [Display(Name = " پست سازمانی")]
    public string PostTitle { get; set; }

    [Display(Name = " پست اصلی")]
    public bool MainPost { get; set; }

    [Display(Name = "نام کاربری")]
    public string PersonalCode { get; set; }

    [Display(Name = " نوع پرسنل در سازمان")]
    public string staffType { get; set; }

    [Display(Name = " نوع پست")]
    public string PostType { get; set; }

    public string ImagePath { get; set; }
}