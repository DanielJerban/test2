namespace BPMS.Domain.Common.ViewModels;

public class FlowsByStaffViewModel
{
    public string FlowId { get; set; }
    public long RequestNo { get; set; }
    public string RequestType { get; set; }
    public string FlowTitle { get; set; }
    public string RegisterDate { get; set; }
    public string RegisterTime { get; set; }
    public string RequesterName { get; set; }
    public string Version { get; set; }

    public StaffDropDownViewModel StaffDropDown { get; set; }
}

public class StaffDropDownViewModel
{
    public string value { get; set; }
    public string text { get; set; }
}