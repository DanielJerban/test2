using BPMS.Domain.Common.Dtos;

namespace BPMS.Domain.Common.ViewModels;

public class ChangeFlowViewModel
{
    public IEnumerable<SelectListItem> AllStaff { get; set; }
    public Guid OldStaffId { get; set; }
    public IEnumerable<SelectListItem> ActiveStaff { get; set; }
    public IEnumerable<FlowViewModel> NewFlow { get; set; }

}