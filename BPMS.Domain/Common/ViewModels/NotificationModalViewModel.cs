namespace BPMS.Domain.Common.ViewModels;

public class FlowDetailsForNotificationModalViewModel
{
    public string PersonelCode { get; set; }
    public string Dsr { get; set; }
    public string RequestDate { get; set; }
    public string RequestTime { get; set; }
    public string Status { get; set; }
    public long RequestNo { get; set; }
    public string CurrentLevel { get; set; }
    public double WaitingTime { get; set; }
    public string FullName { get; set; }
}

public class RequestDetailsForNotificationModalViewModel
{
    public string PersonelCode { get; set; }
    public string Dsr { get; set; }
    public string RequestDate { get; set; }
    public string RequestTime { get; set; }
    public string Status { get; set; }
    public long RequestNo { get; set; }
    public string CurrentLevel { get; set; }
    public string FullName { get; set; }
}

public class NotificationModalViewModel
{
    public RequestDetailsForNotificationModalViewModel RequestDetails { get; set; }
    public FlowDetailsForNotificationModalViewModel FlowDetails { get; set; }
    public bool IsRequestType { get; set; }
}