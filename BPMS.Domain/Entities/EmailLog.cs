using System.ComponentModel.DataAnnotations;

namespace BPMS.Domain.Entities;

public class EmailLog
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string? SenderEmail { get; set; }
    public string? RecieverEmail { get; set; }
    public int SentDate { get; set; }
    [MaxLength(4)]
    public string? Time { get; set; }
    public string? EmailText { get; set; }
    public bool SentStatus { get; set; }
    public string? ErrorMessage { get; set; }
}