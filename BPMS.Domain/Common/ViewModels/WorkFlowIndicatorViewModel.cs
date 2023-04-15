using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using BPMS.Domain.Common.Dtos;

namespace BPMS.Domain.Common.ViewModels;

public class WorkFlowIndicatorViewModel
{
    public Guid Id { get; set; }

    [Display(Name = "عنوان شاخص")]
    [Required(ErrorMessage = "{0} وارد نشده است")]
    public string Title { get; set; }

    [Display(Name = "فرآیند")]
    [Required(ErrorMessage = "{0} وارد نشده است")]
    public Guid RequestTypeId { get; set; }

    [Display(Name = "فرآیند")]
    public string RequestType { get; set; }

    [Display(Name = "فعالیت")]
    [Required(ErrorMessage = "{0} وارد نشده است")]
    public Guid ActivityId { get; set; }

    [Display(Name = "فعالیت")]
    public string Activity { get; set; }

    [Display(Name = "بازه زمانی")]
    [Required(ErrorMessage = "{0} وارد نشده است")]
    public Guid DurationId { get; set; }
    [Display(Name = "بازه زمانی")]

    public string Duration { get; set; }
    [Display(Name = "وضعیت")]
    [Required(ErrorMessage = "{0} وارد نشده است")]
    public Guid FlowstatusId { get; set; }
    [Display(Name = "وضعیت")]

    public string Flowstatus { get; set; }
    [Display(Name = "معیار محاسبه")]
    [Required(ErrorMessage = "{0} وارد نشده است")]

    public Guid CalcCriterionId { get; set; }
    [Display(Name = "معیار محاسبه")]

    public string CalcCriterion { get; set; }
    [Display(Name = "شروع هشدار")]
    [Required(ErrorMessage = "{0} وارد نشده است")]
    public int Warning { get; set; }
    [Display(Name = "شروع بحران")]
    [Required(ErrorMessage = "{0} وارد نشده است")]
    public int Crisis { get; set; }

    public Guid? WidgetTypeId { get; set; }
    [DisplayName("شاخص")]
    [Required(ErrorMessage = "{0} وارد نشده است")]
    public string WidgetType { get; set; }
    [Required(ErrorMessage = "{0} وارد نشده است")]
    [DisplayName("گروه ابزار")]
    public string WidgetGroupTypeId { get; set; }
    [DisplayName("گروه ابزار")]
    public string WidgetGroupType { get; set; }
    public int RegisterDate { get; set; }
    [MaxLength(4)]
    public string RegisterTime { get; set; }

    public IEnumerable<SelectListItem> RequestTypeListItems { get; set; }
    public IEnumerable<SelectListItem> ActivityListItems { get; set; }
    public IEnumerable<SelectListItem> DurationListItems { get; set; }
    public IEnumerable<SelectListItem> FlowStatusListItems { get; set; }
    public IEnumerable<SelectListItem> CalcCriterionListItems { get; set; }
    public IEnumerable<SelectListItem> WidgetGroupTypeListItem { get; set; }

}