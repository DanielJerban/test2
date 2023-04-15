using System.ComponentModel.DataAnnotations;
using BPMS.Domain.Common.CustomFilters;

namespace BPMS.Domain.Common.ViewModels;

public class ClientViewModel
{
    public Guid? Id { get; set; }

    [MaxLength(30, ErrorMessage = "حداکثر {1} کاراکتر باید وارد شود")]
    [Required(ErrorMessage = "{0} وارد نشده است")]
    [Display(Name = "نام")]
    public string FName { get; set; }

    [MaxLength(40, ErrorMessage = "حداکثر {1} کاراکتر باید وارد شود")]
    [Required(ErrorMessage = "{0} وارد نشده است")]
    [Display(Name = "نام خانوادگی")]
    public string LName { get; set; }

    [MaxLength(10, ErrorMessage = "حداکثر {1} کاراکتر باید وارد شود")]
    [Display(Name = "کد ملی")]
    [NationalCodeValidation]
    [Required(ErrorMessage = "{0} وارد نشده است")]
    public string NationalNo { get; set; }

    [MaxLength(100, ErrorMessage = "حداکثر {1} کاراکتر باید وارد شود")]
    [Display(Name = "توضیحات سازمان متبوع")]
    public string FromDsr { get; set; }

    [Display(Name = "آدرس")]
    public string Address { get; set; }

    [MaxLength(20, ErrorMessage = "حداکثر {1} کاراکتر باید وارد شود")]
    [Display(Name = "تلفن همراه")]
    [Required(ErrorMessage = "{0} وارد نشده است")]
    [MobileValidationAttribute]
    public string CellPhone { get; set; }

    [Display(Name = "توضیحات")]
    public string Dsr { get; set; }

    [Display(Name = "فعال")]
    public bool Active { get; set; }

    [Display(Name = "ایمیل")]
    [MaxLength(200, ErrorMessage = "حداکثر {1} کاراکتر باید وارد شود")]
    [DataType(DataType.EmailAddress, ErrorMessage = "آدرس ایمیل وارد شده معتبر نمی باشد")]
    public string Email { get; set; }

    [MaxLength(50, ErrorMessage = "حداکثر {1} کاراکتر باید وارد شود")]
    [Display(Name = "پست سازمانی")]
    public string OrganizationPost { get; set; }

    [Display(Name = "نام و نام خانوادگی")]
    public string FullName => FName + " " + LName;
}