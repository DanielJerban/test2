namespace BPMS.Domain.Common.ViewModels;

public class SubmitChangeRequestViewModel
{
    public string RequestId { get; set; }
    public string StaffId { get; set; }
    public bool IsRemoved { get; set; }
    public bool IsIgnored { get; set; }
}