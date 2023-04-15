using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using BPMS.Domain.Common.Dtos;

namespace BPMS.Domain.Common.ViewModels;

public class WorkFlowFormViewModel
{
    public string Mode { get; set; }
    public Guid Id { get; set; }
    public Guid? PreviousId { get; set; }

    [Required(ErrorMessage = "{0} وارد نشده است")]
    [Display(Name = "نام")]
    public string PName { get; set; }
    public byte[] Content { get; set; }

    public string JsonFile { get; set; }
    public Guid? StaffId { get; set; }

    [DisplayName("نام ایجاد کننده")]
    public string Staff { get; set; }

    [DisplayName("آخرین تغییر دهنده")]
    public string Modifier { get; set; }

    [DisplayName("تاریخ ایجاد")]
    public string RegisterDateTime { get; set; }

    [DisplayName("آخرین تاریخ تغییر")]
    public string ModifiedDateTime { get; set; }
    public string Jquery { get; set; }
    public string AdditionalCssStyleCode { get; set; }
    public string Value { get; set; }
    public bool? IsCopy { get; set; }
    public Guid uniqeid { get; set; }
    public Guid RequestId { get; set; }
    public string Status { get; set; }
    public List<PreviousFormViewModel> PreviousForm { get; set; }
    public string EditableCheck { get; set; }
    public string HiddenFields { get; set; }
    public Guid WorkFlowDetailId { get; set; }
    public FlowDetailsInfo DetailsInfo { get; set; }
    public FlowParam Param { get; set; }
    public string PrintFileName { get; set; }

    [Display(Name = "کد سند")]
    public string DocumentCode { get; set; }
    public bool IsSavedForm { get; set; }


    [Display(Name = "نسخه اصلی")]
    [Required(ErrorMessage = "{0} را وارد نمایید.")]
    [Range(0, 100, ErrorMessage = "نسخه اصلی باید بین 0 تا 100 باشد")]
    public int OrginalVersion { get; set; }

    [Display(Name = "نسخه فرعی")]
    [Required(ErrorMessage = "{0} را وارد نمایید.")]
    [Range(0, 9, ErrorMessage = "نسخه فرعی باید بین 0 تا 9 باشد")]
    public int SecondaryVersion { get; set; }

    [Display(Name = "نسخه")]
    public string Version => $"{OrginalVersion},{SecondaryVersion}";
    public string Dsr { get; set; }
    public string WorkFlowAbout { get; set; }
    public string TutorialFileName { get; set; }
    public string StepTitle { get; set; }
    public IEnumerable<SelectListItem> AdHocWorkflowDetails { get; set; }
    public Guid AdHocWorkFlowId { get; set; }
    public bool IsAdHoc { get; set; }
    public bool NoReject { get; set; }
    public StaffViewModel StaffIdForMobile { get; set; }
    public string ReturnUrlForWeb { get; set; }
    public bool IsItFromReportView { get; set; } = false;

    public string FullFormData { get; set; }

    public int SortingRegisterDate { get; set; }

    public int SortingRegisterTime { get; set; }
    public string Time { get; set; }

}

public class PreviousFormViewModel
{
    public string FormId { get; set; }
    public string JsonForm { get; set; }
    public string ValueForm { get; set; }
}