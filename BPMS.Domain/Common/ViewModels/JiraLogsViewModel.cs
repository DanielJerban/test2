using System.ComponentModel.DataAnnotations;

namespace BPMS.Domain.Common.ViewModels;

public class JiraLogsViewModel
{
    public Guid Id { get; set; }

    [Display(Name = "نام کاربری")]
    public string UserName { get; set; }

    [Display(Name = "تاریخ")]
    public string Date { get; set; }

    [Display(Name = "زمان")]
    public string Time { get; set; }

}