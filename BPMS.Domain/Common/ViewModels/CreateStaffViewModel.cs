using System.ComponentModel.DataAnnotations;
using BPMS.Domain.Common.Dtos;
using Microsoft.AspNetCore.Http;

namespace BPMS.Domain.Common.ViewModels;

public class CreateStaffViewModel
{
    public Guid Id { get; set; }

    [MaxLength(10, ErrorMessage = "نام کاربری  نمی تواند بیش از 10 کاراکتر باشد")]
    [Required(ErrorMessage = "{0} را وارد نمایید")]
    [Display(Name = "نام کاربری")]
    [RegularExpression(@"^[0-9]*$", ErrorMessage = "فرمت نام کاربری وارد شده نادرست است")]
    public string PersonalCode { get; set; }

    [Required(ErrorMessage = "{0} را وارد نمایید")]
    [Display(Name = "نام")]
    public string FName { get; set; }

    [Required(ErrorMessage = "{0} را وارد نمایید")]
    [Display(Name = "نام خانوادگی")]
    public string LName { get; set; }

    [Display(Name = "شماره موبایل")]
    [Required(ErrorMessage = "{0} را وارد نمایید")]
    [RegularExpression(@"^[0][9][0-3][0-9]{8,8}$", ErrorMessage = "شماره تماس وارد شده معتبر نیست")]
    [MaxLength(11, ErrorMessage = "حداقل 11 کاراکتر باید وارد کنید")]
    [MinLength(11)]
    public string PhoneNumber { get; set; }

    [Display(Name = "آدرس ایمیل")]
    [Required(ErrorMessage = "{0} را وارد نمایید")]
    [DataType(DataType.EmailAddress, ErrorMessage = "آدرس ایمیل وارد شده معتبر نیست")]

    public string Email { get; set; }

    [Display(Name = "تصویر پروفایل")]
    public string ImagePath { get; set; }

    [Display(Name = "نوع پرسنل")]
    [Required(ErrorMessage = "{0} را وارد نمایید")]
    public Guid StaffTypeId { get; set; }

    [Display(Name = "شماره بیمه")]
    [Required(ErrorMessage = "{0} را وارد نمایید")]
    public string InsuranceNumber { get; set; }

    [Display(Name = "شماره شناسنامه")]
    [Required(ErrorMessage = "{0} را وارد نمایید")]
    public string IdNumber { get; set; }

    [Required(ErrorMessage = "{0} را وارد نمایید")]
    [RegularExpression(@"^(?=.*[A-Za-z])(?=.*[0-9]).{6,}$", ErrorMessage = "رمز عبور باید حداقل 6 کارکتر و شامل اعداد و حروف باشد")]
    [DataType(DataType.Password)]
    [Display(Name = "رمز عبور")]
    public string Password { get; set; }

    [Required(ErrorMessage = "{0} را وارد نمایید")]
    [RegularExpression(@"^(?=.*[A-Za-z])(?=.*[0-9]).{6,}$", ErrorMessage = "تکرار رمز عبور باید حداقل 6 کارکتر و شامل اعداد و حروف باشد")]
    [DataType(DataType.Password)]
    [System.ComponentModel.DataAnnotations.Compare(nameof(Password), ErrorMessage = "{0} و {1} برابر نیستند.")]
    [Display(Name = "تکرار رمز عبور")]
    public string ConfirmPassword { get; set; }

    [Display(Name = "LDAP User Name")]
    public string LDAPUserName { get; set; }
    [Display(Name = "LDAP Domain")]
    public string LDAPDomainName { get; set; }


    public List<SelectListItem> StaffTypeListItems { get; set; }
    public IFormFile Attachment { get; set; }
}