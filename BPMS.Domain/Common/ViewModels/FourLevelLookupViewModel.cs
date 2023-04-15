namespace BPMS.Domain.Common.ViewModels;

public class FourLevelLookupViewModel
{
    public Guid Aux { get; set; }
    public string Idenc { get; set; }

    public IEnumerable<LookUpViewModel> Level1 { get; set; }
    public IEnumerable<LookUpViewModel> Level2 { get; set; }
    public IEnumerable<LookUpViewModel> Level3 { get; set; }

    public IEnumerable<LookUpViewModel> Level1Create { get; set; }
    public IEnumerable<LookUpViewModel> Level1Edit { get; set; }
    public IEnumerable<Guid> Level1Delete { get; set; }

    public IEnumerable<LookUpViewModel> Level2Create { get; set; }
    public IEnumerable<LookUpViewModel> Level2Edit { get; set; }
    public IEnumerable<Guid> Level2Delete { get; set; }

    public IEnumerable<LookUpViewModel> Level3Create { get; set; }
    public IEnumerable<LookUpViewModel> Level3Edit { get; set; }
    public IEnumerable<Guid> Level3Delete { get; set; }

    public string Type1 { get; set; }
    public string Type2 { get; set; }
    public string Type3 { get; set; }


    public string Title1 { get; set; }
    public string Title2 { get; set; }
    public string Title3 { get; set; }


    public string MainTitle { get; set; }
    public string FatherPage { get; set; }
    public string Level2Page { get; set; }
}