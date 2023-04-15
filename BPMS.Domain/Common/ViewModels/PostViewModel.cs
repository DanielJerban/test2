namespace BPMS.Domain.Common.ViewModels;

public class PostViewModel
{
    public Guid? PostId { set; get; }
    public string StaffId { get; set; }
    public string OrganiztionPostId { get; set; }
    public string ChartId { get; set; }
    public string Priority { get; set; }
    public string IsActive { get; set; }

}