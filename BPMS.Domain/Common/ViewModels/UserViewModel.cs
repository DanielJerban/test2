using System.ComponentModel.DataAnnotations;

namespace BPMS.Domain.Common.ViewModels;

public class UserViewModel
{
    public Guid Id { get; set; }
   
    public Guid StaffId { get; set; }

    public Guid RoleAccessId { get; set; }
       
    [MaxLength(50, ErrorMessage = "{0} نباید بیشتر از 50 حرف باشد")]
    [Required(ErrorMessage = "{0} وارد نشده است")]
    [Display(Name = "نام کاربری")]
    public string UserName { get; set; }

    [DataType(DataType.Password)]
    [Required(ErrorMessage = "{0} وارد نشده است")]
    [Display(Name = "کلمه عبور")]
    public string Password { get; set; }

    [Display(Name = "وضعیت")]
    public bool IsActive { get; set; }

    [Display(Name = "نام کاربری")]
    public string PersonelCode { get; set; }

    [Required(ErrorMessage = "{0} وارد نشده است")]
    [Display(Name = "شماره همراه")]
    public string PhoneNumber { get; set; }
        
    [Required(ErrorMessage = "{0} وارد نشده است")]
    [Display(Name = "ایمیل")]
    public string Email { get; set; }

        
    [Display(Name = "نام و نام خانوادگی")]
    public string FullName { get; set; }
        
    [Display(Name = "شماره بیمه")]
    public string InsuranceNumber { get; set; }
        
    [Display(Name = "شماره شناسنامه")]
    public string IdNumber { get; set; }
        
    [Display(Name = "تاریخ استخدام")]
    public int? EmploymentDate { get; set; }
}