using System.ComponentModel.DataAnnotations;

namespace BPMS.Domain.Common.ViewModels;

public class ThirdPartyViewModel
{
    public Guid? Id { get; set; }

    [Required(ErrorMessage = "{0} وارد نشده است")]
    [Display(Name = "نام")]
    public string Name { get; set; }

    [Display(Name = "توضیحات")]
    public string Description { get; set; }

    [Display(Name = "فعال")]
    public bool IsActive { get; set; }

    [Display(Name = "نام کاربری")]
    [Required(ErrorMessage = "{0} وارد نشده است")]
    [RegularExpression(@"^[A-Za-z\d_-]+$", ErrorMessage = "{0} فقط می تواند حرف انگلیسی و عدد باشد")]
    public string UserName { get; set; }

    [Display(Name = "رمز عبور")]
    [Required(ErrorMessage = "{0} وارد نشده است")]
    [MinLength(6, ErrorMessage = "طول رشته نمی تواند کمتر از 6 باشد")]
    public string Password { get; set; }

    [Display(Name = "رمز عبور منقضی می شود؟")]
    public bool PasswordExpires { get; set; }

    [Display(Name = "تاریخ انقضا")]
    public int? ExpireDate { get; set; }

    [Display(Name = "تاریخ انقضا")]
    public string ExpireDateString { get; set; }

    [Display(Name = "IP های معتبر")]
    public string IPAddresses { get; set; }
}