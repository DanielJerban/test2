using System.ComponentModel.DataAnnotations;

namespace BPMS.Domain.Common.ViewModels;

public class LookUpViewModel
{
    public Guid? Id { get; set; }
        
    [Display(Name = "کد")]
    public int Code { get; set; }

    [Display(Name = "دسته بندی")]
    [MaxLength(50)]
    public string Type { get; set; }

    [MaxLength(100)]
    [Display(Name = "عنوان")]
    [Required(ErrorMessage = "لطفا عنوان را وارد کنید")]
    public string Title { get; set; }

    [MaxLength(100)]
    [Display(Name = "وابستگی اول")]
    public string Aux { get; set; }

    [MaxLength(100)]
    [Display(Name = "مقدار")]
    public string Aux2 { get; set; }

    [Display(Name = "فعال")]
    public bool IsActive { get; set; }

    public int Count { get; set; }

    public string SubAux { set; get; }
}