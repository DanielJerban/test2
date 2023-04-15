namespace BPMS.Domain.Common.ViewModels;

public class FormClassificationFilesViewModel
{
    public string Id { get; set; }
    public string FileName { get; set; }
    public string Path { get; set; }
    public string Title { get; set; }
    public string FormClassificationTitle { get; set; }
    public Guid FormId { get; set; }
}