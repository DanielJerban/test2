namespace BPMS.Domain.Common.ViewModels;

public class ReportParam
{
    public string Title { get; set; }
    public string Parent { get; set; }
    public IEnumerable<dynamic> Model { get; set; }
    public Type Type { get; set; }
    public  dynamic Column { get; set; }
    public string PartialName { get; set; }
}