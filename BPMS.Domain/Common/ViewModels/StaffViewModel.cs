using System.ComponentModel.DataAnnotations;

namespace BPMS.Domain.Common.ViewModels;

public class StaffViewModel
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }

    [Display(Name = "نام کاربری")]
    public string PersonalCode { get; set; }

    [Display(Name = "نام کاربری")]
    public string UserName { get; set; }
        
    public string FName { get; set; }
        
    public string LName { get; set; }

    [Display(Name = "شماره همراه")]
    public string PhoneNumber { get; set; }

    [Display(Name = "نام و نام خانوادگی")]
    public string FullName { get; set; }
        
    [Display(Name = "نوع پرسنل")]
    public string StaffType { set; get; }
        
    [Display(Name = "آدرس ایمیل")]
    public string EmailAddress { get; set; }
        
    [Display(Name = "شماره بیمه")]
    public string InsuranceNumber { get; set; }
        
    [Display(Name = "شماره شناسنامه")]
    public string IdNumber { get; set; }
        
    [Display(Name = "تاریخ ورود به شرکت")]
    public int? EmploymentDate { get; set; }
        
    [Display(Name = "ساختمان")]
    public string Building { get; set; }
        
    [Display(Name = "تلفن داخلی")]
    public string LocalPhone  { get; set; }
        
    public string ImagePath { get; set; }
        
    [Display(Name = "عنوان پست سازمانی")]
    public string PostTitle { get; set; }
        
    public string PostType { get; set; }
        
    [Display(Name = "واحد")]
    public string ChartTitle { get; set; }

    [Display(Name = "شرکت")]
    public string CompanyName { get; set; }
}