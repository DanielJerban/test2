using BPMS.Domain.Common.CustomFilters;
using System.ComponentModel.DataAnnotations;

namespace BPMS.Domain.Common.ViewModels;

public class CompanyViewModel
{
    public Guid? Id { get; set; }

    [MinLength(1, ErrorMessage = "{0} وارد نشده است")]
    [Display(Name = "نام شرکت")]
    public string Name { get; set; }

    [MaxLength(20, ErrorMessage = "حداکثر {1} کاراکتر باید وارد شود")]
    [Display(Name = "کد اقتصادی")]
    public string EconomicCode { get; set; }

    [MaxLength(30, ErrorMessage = "حداکثر {1} کاراکتر باید وارد شود")]
    [Display(Name = "نام اختصاری")]
    public string ShortName { get; set; }

    [MaxLength(13, ErrorMessage = "حداکثر {1} کاراکتر باید وارد شود")]
    [Display(Name = "تلفن")]
    [PhoneNumberValidation]
    public string Telephone { get; set; }
    [MaxLength(30, ErrorMessage = "حداکثر {1} کاراکتر باید وارد شود")]
    [Display(Name = "فکس")]
    public string Fax { get; set; }

    [Display(Name = "پست الکترونیک")]
    [MaxLength(30, ErrorMessage = "حداکثر {1} کاراکتر باید وارد شود")]
    [DataType(DataType.EmailAddress, ErrorMessage = "آدرس ایمیل وارد شده معتبر نمی باشد")]
    public string Email { get; set; }

    [Display(Name = "وب سایت")]
    [MaxLength(100, ErrorMessage = "حداکثر {1} کاراکتر باید وارد شود")]
    [RegularExpression(@"^(http|http(s)?://)?([\w-]+\.)+[\w-]+[.com|.in|.org]+(\[\?%&=]*)?", ErrorMessage = "وبسایت وارد شده معتبر نمی باشد")]
    public string WebSite { get; set; }

    [MaxLength(20, ErrorMessage = "حداکثر {1} کاراکتر باید وارد شود")]
    [Display(Name = "کد پستی")]
    public string PostalCode { get; set; }

    [MaxLength(500, ErrorMessage = "حداکثر {1} کاراکتر باید وارد شود")]
    [Display(Name = "آدرس")]
    public string FullAddress { get; set; }

    [Display(Name = "توضیحات")]
    public string Dsr { get; set; }

    [Display(Name = "تاریخ ثبت در سیستم")]
    public long RegisterDate { get; set; }

    [Display(Name = "شناسه ملی")]
    [MaxLength(20, ErrorMessage = "حداکثر {1} کاراکتر باید وارد شود")]
    public string NationalCode { get; set; }

    [MaxLength(20, ErrorMessage = "حداکثر {1} کاراکتر باید وارد شود")]
    [Display(Name = "شماره ثبت")]
    public string RegistrationNo { get; set; }
    public string RegisterTime { get; set; }
    public DateTime AddDate { get; set; }
}