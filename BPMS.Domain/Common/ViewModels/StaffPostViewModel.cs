namespace BPMS.Domain.Common.ViewModels;

public class StaffPostViewModel
{
    public Guid OrgId { get; set; }
    public Guid ChartId { get; set; }
    public string StaffId { get; set; }
    public string ChartTitle { get; set; }
    public string PostType { get; set; }
    public string PostId { get; set; }
    public string PostTitle { get; set; }
    public bool Status { get; set; }
    public bool MainPost { get; set; }
}