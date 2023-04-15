using System.ComponentModel.DataAnnotations;

namespace BPMS.Domain.Common.ViewModels;

public class FormLookup2NViewModel
{
    public Guid Id { get; set; }

    [Required(ErrorMessage = "{0} وارد نشده است")]
    [Display(Name = "عنوان اصلی")]
    public string Title { get; set; }

    [Required(ErrorMessage = "{0} وارد نشده است")]
    [Display(Name = "عنوان مرحله اول")]
    public string Title1 { get; set; }

    [Required(ErrorMessage = "{0} وارد نشده است")]
    [Display(Name = "عنوان مرحله دوم")]
    public string Title2 { get; set; }

    [Required(ErrorMessage = "{0} وارد نشده است")]
    [Display(Name = "نوع مرحله اول")]
    public string Type1 { get; set; }

    [Required(ErrorMessage = "{0} وارد نشده است")]
    [Display(Name = "نوع مرحله دوم")]
    public string Type2 { get; set; }
}