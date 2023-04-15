using System.ComponentModel.DataAnnotations;
using BPMS.Domain.Common.CustomFilters;

namespace BPMS.Domain.Common.ViewModels;

public class UserProfileInfoViewModel
{
    [Display(Name = "نام کاربری")]
    public string PersonalCode { get; set; }


    [Display(Name = "نام")]
    public string FName { get; set; }

    [Display(Name = "نام خانوادگی")]
    public string LName { get; set; }

    [Display(Name = "شماره موبایل")]
    [MobileValidation(ErrorMessage = "فرمت موبایل نادرست است")]
    public string PhoneNumber { get; set; }

    [Display(Name = "آدرس ایمیل")]
    [EmailValidationAttribute(ErrorMessage = "فرمت ایمیل نادرست است")]
    public string Email { get; set; }

    [Display(Name = "تصویر پروفایل")]
    public string ImagePath { get; set; }
        
    [Display(Name = "نوع پرسنل")]
    public Guid StaffTypeId { get; set; }

    [Display(Name = "نوع پرسنل")]
    public string StaffTypeTitle { get; set; }

    [Display(Name = "شماره بیمه")]
    public string InsuranceNumber { get; set; }

    [Display(Name = "شماره شناسنامه")]
    public string IdNumber { get; set; }


    [Display(Name = "نقش های کاربر")]
    public List<string> UserRoles { get; set; }
        
    [Display(Name = "لاگین دو مرحله ای با پیامک")]
    public bool TwoStepVerification { get; set; }
        
    [Display(Name = "لاگین دو مرحله ای با ایمیل")]
    public bool TwoStepVerificationByEmail { get; set; }
       
    [Display(Name = "لاگین دو مرحله ای با احراز هویت گوگل ")]
    public bool TwoStepVerificationByGoogleAuthenticator { get; set; }

}