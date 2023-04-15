namespace BPMS.Domain.Common.ViewModels;

public class ProccessStatusViewModel
{
    public Guid RequestId { get; set; }
    public string RequestTypeTitle { get; set; }
    public Guid RequestTypeId { get; set; }
    public int T { get; set; }
}