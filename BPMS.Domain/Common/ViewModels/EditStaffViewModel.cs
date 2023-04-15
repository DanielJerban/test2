using System.ComponentModel.DataAnnotations;
using BPMS.Domain.Common.Dtos;

namespace BPMS.Domain.Common.ViewModels;

public class EditStaffViewModel
{
    public string PersonalCode { get; set; }

    [Required(ErrorMessage = "{0} را وارد نمایید")]
    [Display(Name = "نام")]
    public string FName { get; set; }

    [Required(ErrorMessage = "{0} را وارد نمایید")]
    [Display(Name = "نام خانوادگی")]
    public string LName { get; set; }

    [Required(ErrorMessage = "{0} را وارد نمایید")]
    [Display(Name = "شماره موبایل")]
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
    [Required(ErrorMessage = "{0} وارد نشده است")]
    public Guid StaffTypeId { get; set; }

    public string StaffTypeTitle { get; set; }

    [Display(Name = "شماره بیمه")]
    [Required(ErrorMessage = "{0} را وارد نمایید")]
    public string InsuranceNumber { get; set; }

    [Display(Name = "شماره شناسنامه")]
    [Required(ErrorMessage = "{0} را وارد نمایید")]
    public string IdNumber { get; set; }

    [Display(Name = "LDAP User Name")]
    public string LDAPUserName { get; set; }

    [Display(Name = "LDAP Domain")]
    public string LDAPDomainName { get; set; }

    public List<SelectListItem> StaffTypeListItems { get; set; }

    [Display(Name = "نقش های کاربر")]
    public List<string> UserRoles { get; set; }
}