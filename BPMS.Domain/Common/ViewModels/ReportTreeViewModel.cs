namespace BPMS.Domain.Common.ViewModels;

public class ReportTreeViewModel
{
    public string Id { get; set; }
    public string Text { get; set; }
    public string ImageUrl { get; set; }
    public bool Expanded { get; set; }
    public string SpriteCssClass { get; set; }
    public string Type { get; set; }
    public string Table { get; set; }
    public List<ReportTreeViewModel> Items { get; set; }
    public bool Check { get; set; }
    public string Options { get; set; }
}