namespace BPMS.Domain.Common.ViewModels;

public class EditPatternViewModel
{
    public List<string> selectedPosts { get; set; }
    public string patternName { get; set; }
    public Guid Id { get; set; }
}