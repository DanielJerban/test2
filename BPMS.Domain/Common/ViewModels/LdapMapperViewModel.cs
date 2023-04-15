using System.ComponentModel.DataAnnotations;

namespace BPMS.Domain.Common.ViewModels;

public class LdapMapperViewModel
{
    [Display(Name = "نام کاربری")]
    public string LdapUserName { get; set; }

    [Display(Name = "کد پرسنلی")]
    public string PersonalCode { get; set; }
        
    [Display(Name = "نام")]
    public string FName { get; set; }
        
    [Display(Name = "نام خانوادگی")]
    public string LName { get; set; }
        
    [Display(Name = "ایمیل")]
    public string Email { get; set; }
        
    [Display(Name = "شماره موبایل")]
    public string PhoneNumber { get; set; }
        
    [Display(Name = "شماره بیمه")]
    public string InsuranceNumber { get; set; }
        
    [Display(Name = "شماره شناسنامه")]
    public string IdNumber { get; set; }
        
    [Display(Name = "شماره داخلی")]
    public string LocalPhone { get; set; }
        
    [Display(Name = "تصویر پروفایل")]
    public string ImagePath { get; set; }
        
    [Display(Name = "رمز عبور")]
    public string UserPassword { get; set; }
}