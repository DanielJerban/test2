namespace BPMS.Domain.Common.ViewModels;

public class OptimisedAccessViewModel
{
    public List<ChartViewModel> Charts { get; set; }
    public List<UserViewModel> Users { get; set; }
    public List<PostTypeViewModel> PostTypes { get; set; }
    public List<PostTitleViewModel> PostTitles { get; set; }
    public AccessViewModel AccessViewModel { get; set; }
    public object Data { get; set; }
}