using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace BPMS.Domain.Common.ViewModels;

public class WorkFlowViewModel
{
    public Guid Id { get; set; }
    public Guid IdForCopy { get; set; }

    [Display(Name = "نام فرآیند")]
    [Required(ErrorMessage = "{0} وارد نشده است")]
    public string RequestType { get; set; }
        
    [DisplayName("گروه فرآیند")]
    public string RequestGroupType { get; set; }
        
    [Required(ErrorMessage = "{0} وارد نشده است")]
    [DisplayName("گروه فرآیند")]
    public Guid? RequestGroupTypeId { get; set; }
        
    public Guid RequestTypeId { get; set; }
        
    public Guid StaffId { get; set; }
        
    [Display(Name = "نام ایجاد کننده")]
    public string Staff { get; set; }

    [DisplayName("آخرین تغییر دهنده")]
    public string Modifier { get; set; }

    [DisplayName("تاریخ ایجاد")]
    public string RegisterDateTime { get; set; }

    [DisplayName("آخرین تاریخ تغییر")]
    public string ModifiedDateTime { get; set; }

    [Display(Name = "فعال")]
    public bool IsActive { get; set; }
        
    [Display(Name = "نسخه اصلی")]
    [Required(ErrorMessage = "{0} وارد نشده است")]
    [Range(1, 100, ErrorMessage = "{0} باید بین {1} تا {2} باشد.")]
    public int OrginalVersion { get; set; }
        
    [Display(Name = "نسخه فرعی")]
    [Range(0, 9, ErrorMessage = "{0} باید بین {1} تا {2} باشد.")]
    [Required(ErrorMessage = "{0} وارد نشده است")]
    public int SecondaryVersion { get; set; }
        
    [Display(Name = "توضیحات")]
    public string Dsr { get; set; }
        
    [Display(Name = "شرح فرآیند")]
    public string About { get; set; }

    [Display(Name = "نسخه")]
    public string Version { get; set; }
        
    public Guid FlowTypeId { get; set; }
        
    [Display(Name = "نوع ذخیره سازی")]
    public string FlowType { get; set; }

    public IList<string> DataElementId { get; set; }
        
    public string XmlFile { get; set; }
        
    public byte[] Content { get; set; }
        
    public int? WaitingTime { get; set; }
        
    [Display(Name = "کلمات کلیدی")]
    public string KeyWords { get; set; }
        
    public Guid? SubProcessId { get; set; }
        

    [Display(Name = "نام صاحب فرایند")]
    [Required(ErrorMessage = "{0} وارد نشده است")]
    public string Owner { get; set; }

    public bool inActiveProcess { get; set; }

    [Display(Name = "شناسه خارجی")]
    [Required(ErrorMessage = "{0} وارد نشده است")]
    public Guid? ExternalId { get; set; }

    public int SortingRegisterDate { get; set; }

    public int SortingRegisterTime { get; set; }
    public string Time { get; set; }

}