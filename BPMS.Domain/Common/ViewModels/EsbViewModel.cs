namespace BPMS.Domain.Common.ViewModels;

public class EsbViewModel
{
    public string EmailSubject { get; set; }
    public string EmailBody { get; set; }
    public string EmailRecieve { get; set; }
    public string SmsText { get; set; }
    public string SmsRecieve { get; set; }
    public string SendRemoteId { get; set; }
    public string DynamicGetCode { get; set; }
    public string FormIdForMessage { get; set; }
    public bool SmsRequester { get; set; }
    public bool EmailRequester { get; set; }
    public FlowParam Param { get; set; }
}