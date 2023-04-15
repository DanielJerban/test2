using System.ComponentModel.DataAnnotations;

namespace BPMS.Domain.Common.ViewModels;

public class RoleViewModel
{
    public Guid? Id { get; set; }

    [Display(Name = "نام گروه")]
    [Required(ErrorMessage = "{0} وارد نشده است")]
    public string Name { get; set; }
        
    [Display(Name = "توضیحات")]
    public string Description { get; set; }
}