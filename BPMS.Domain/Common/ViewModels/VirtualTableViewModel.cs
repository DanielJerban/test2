using System.ComponentModel.DataAnnotations;

namespace BPMS.Domain.Common.ViewModels;

public class VirtualTableViewModel
{
    public Guid? Id { get; set; }
        
    public int Code { get; set; }

    public string Type { get; set; }

    [Display(Name = "عنوان")]
    [Required(ErrorMessage = "{0} وارد نشده است")]
    public string Title { get; set; }
        
    [MaxLength(100,ErrorMessage = "نباید بیش از 100 حرف وارد شود")]
    [Display(Name = "نوع")]
    public string Aux { get; set; }
        
    [Display(Name = "سیستم")]
    [Range(1, int.MaxValue, ErrorMessage = "{0} انتخاب نشده است")]
    public Aux2 Aux2 { get; set; }
        
    public bool IsActive { get; set; }
        
    public bool IsBpmsForm { get; set; }
}

public enum Aux2
{
    HR=1,
    BpmsForm=2,
    FormClassification=3,
    Base=4,
    Schedule=5
}