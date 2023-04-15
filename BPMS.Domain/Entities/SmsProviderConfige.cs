namespace BPMS.Domain.Entities;

public class SmsProviderConfige
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public bool IsActive { get; set; }
    public string? Name { get; set; }
    public string? UserName { get; set; }
    public string? Password { get; set; }
    public string? ApiKey { get; set; }
    public string? ProviderNumber { get; set; }
    public string? Uri { get; set; }
    public int SmsSendType { get; set; }
    public string? GsmPort { get; set; }
    public string? GsmPortRate { get; set; }
    public string? GsmPortReadTimeout { get; set; }
    public string? GsmPortWriteTimeout { get; set; }
}