namespace BPMS.Domain.Common.ViewModels;

public class StaffRequestsViewModel
{
    public Guid RequestTypeId { get; set; }
    public Guid RequestId { get; set; }
    public Guid FlowId { get; set; }
    public long RequestNo { get; set; }
    public string RequestType { get; set; }
    public string RequestDate { get; set; }
    public string RequestTime { get; set; }
    public string CurrentStatus { get; set; }
    public string IgnoreOrgInfChange { get; set; }

    public string Remove { get; set; } = "خیر";
    public StaffDropDownViewModel StaffDropDown { get; set; }
}