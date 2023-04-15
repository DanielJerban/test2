namespace BPMS.Domain.Common.ViewModels;

public class ResponseGroupViewModel
{
    public IEnumerable<LookUpViewModel> OneLevelCreate { get; set; }
    public IEnumerable<LookUpViewModel> OneLevelEdit { get; set; }
    public IEnumerable<Guid> staffIds { get; set; }
    public Guid bpmsGroupId { get; set; }
}