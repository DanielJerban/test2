using BPMS.Domain.Common.Dtos;

namespace BPMS.Domain.Common.ViewModels;

public class CarbotViewModel
{

    public List<SelectListItem> RequestTypeListItems { get; set; }
    public List<SelectListItem> PostTitlesListItems { get; set; }

    public Guid RequestTypeId { get; set; }
    public Guid OrganizationPostTitleId { get; set; }
    public IEnumerable<SelectListItem> RecievedItems { get; set; }
    public IEnumerable<SelectListItem> SendItems { get; set; }

}