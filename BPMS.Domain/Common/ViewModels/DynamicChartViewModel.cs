using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using BPMS.Domain.Common.Dtos;

namespace BPMS.Domain.Common.ViewModels;

public class DynamicChartViewModel
{

    public Guid Id { get; set; }


    public Guid WidgetTypeId { get; set; }
    [DisplayName("نمودار گزارش")]
    [Required(ErrorMessage = "{0} وارد نشده است")]
    public string WidgetType { get; set; }
    [Required(ErrorMessage = "{0} وارد نشده است")]
    [DisplayName("گروه ابزار")]
    public Guid WidgetGroupTypeId { get; set; }
    public string WidgetGroupType { get; set; }
    [Required(ErrorMessage = "{0} وارد نشده است")]
    [DisplayName("گزارش")]
    public Guid ReportId { get; set; }
    [DisplayName("گزارش")]
    public string Report { get; set; }
    [DisplayName("فعال")]
    public bool IsActive { get; set; }
    public string DataSetting { get; set; }
    [DisplayName("نام ایجاد کننده")]
    public string Creator { get; set; }

    public bool IsEdit { get; set; }
    public IEnumerable<SelectListItem> WidgetGroupTypeListItem { get; set; }
    public IEnumerable<SelectListItem> ReportListItem { get; set; }
}