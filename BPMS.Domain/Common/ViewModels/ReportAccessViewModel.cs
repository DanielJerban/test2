using System.ComponentModel.DataAnnotations;

namespace BPMS.Domain.Common.ViewModels;

public class ReportAccessViewModel
{
    public Guid UserId { get; set; }
    public Guid RoleId { get; set; }
    [Display(Name = "نام کاربری")]
    public string UserName { get; set; }
    [Display(Name = "وضعیت")]
    public bool IsActive { get; set; }
    [Display(Name = "نام نقش")]
    public string RoleName { get; set; }
    [Display(Name = "نام و نام خانوادگی")]
    public  string FullName { get; set; }
    [Display(Name = "توصیف نقش")]
    public string RoleDescription { get; set; }
    [Display(Name = "نام کاربری")]
    public string PersonelCode { get; set; }
    [Display(Name = "شماره همراه")]
    public string PhoneNumber { get; set; }
    [Display(Name = "نام کنترلر")]
    public string ControlerName { get; set; }
    [Display(Name = "نام اکشن")]
    public string ActionName { get; set; }
}