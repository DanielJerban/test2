namespace BPMS.Domain.Common.ViewModels;

public class SendNotificationViewModel
{
    public IEnumerable<string> StaffsToNotify { get; set; }
    public string RequestTitle { get; set; }
    public string RequesterStaff { get; set; }
    public string RequestNo { get; set; }
    public string RequesterPersonelCode { get; set; }
    public List<NotificationViewModel> FlowStaffs { get; set; }
    public Guid FlowId { get; set; }
}

public class NotificationViewModel
{
    public string UserName { get; set; }
    public Guid FlowId { get; set; }
}