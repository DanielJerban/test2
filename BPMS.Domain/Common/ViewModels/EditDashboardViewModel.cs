using System.ComponentModel.DataAnnotations;
using BPMS.Domain.Common.Dtos;

namespace BPMS.Domain.Common.ViewModels;

public class EditDashboardViewModel
{
    [Display(Name = "نوع ابزارک")]
    public Guid WidgetTypeId { get; set; }
    public IEnumerable<SelectListItem> WidgetTypeListItems { get; set; }
}

public class WidgetViewModel
{
    public Guid id { get; set; }
    public int x { get; set; }
    public int y { get; set; }
    public int width { get; set; }
    public int height { get; set; }
}

public class WidgetListViewModel
{
    public IEnumerable<StaffPostViewModel> PostInformations { get; set; }
}