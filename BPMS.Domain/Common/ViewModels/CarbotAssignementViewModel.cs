using System.ComponentModel.DataAnnotations;
using BPMS.Domain.Common.Dtos;

namespace BPMS.Domain.Common.ViewModels;

public class CarbotAssignementViewModel
{
    [Display(Name = "در کارتابل من نیز نشان داده شود")]
    public Guid RequestTypeId { get; set; }
    public Guid SelectedMasterId { get; set; }
    public IEnumerable<RequestTypeDropDownViewModel> RequestTypeListItems { get; set; }

    public IEnumerable<CreateAssignementViewModel> DetailsCreate { get; set; }
    public IEnumerable<WorkFlowConfermentAuthorityViewModel> MastersCreate { get; set; }
    public IEnumerable<Guid> MasteRecordsToDelete { get; set; }
    public IEnumerable<Guid> DetailRecordsToDelete { get; set; }
    public IEnumerable<SelectListItem> UsersDropDown { get; set; }

}