namespace BPMS.Domain.Common.ViewModels;

public class OneLevelLookUpViewModel
{
    public string Idenc { get; set; }

    public IEnumerable<LookUpViewModel> OneLevelCreate { get; set; }
    public IEnumerable<LookUpViewModel> OneLevel { get; set; }
    public IEnumerable<LookUpViewModel> OneLevelEdit { get; set; }
    public IEnumerable<Guid> OneLevelDelete { get; set; }
    public string SubAux { set; get; }


    public string System { get; set; }
    public string Title { get; set; }
    public string FatherPage { get; set; }
    public string Level2 { get; set; }
}