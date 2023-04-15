namespace BPMS.Domain.Common.ViewModels;

public class ChartDiagramViewModel
{
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string PersonalCode { get; set; }
    public string Title { get; set; }
    public string ColorScheme { get; set; }
    public List<ChartDiagramViewModel> Items { get; set; }

    public Guid ChartId { get; set; }
}