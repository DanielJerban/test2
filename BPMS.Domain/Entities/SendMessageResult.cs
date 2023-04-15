namespace BPMS.Domain.Entities;

public class SendMessageResult
{
    public bool IsSucceed { get; set; } = false;
    public string Message { get; set; }
    public string ErrorMessage { get; set; }
}