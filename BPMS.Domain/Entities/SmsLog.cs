using System.ComponentModel.DataAnnotations;

namespace BPMS.Domain.Entities;

public class SmsLog
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string? SenderNumber { get; set; }
    public string? RecieverNumber { get; set; }
    public int SentDate { get; set; }
    [MaxLength(4)]
    public string? Time { get; set; }
    public string? SmsText { get; set; }
    public bool SentStatus { get; set; }
    public string? ErrorMessage { get; set; }
    public int SmsSendType { get; set; }
}