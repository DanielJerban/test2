namespace BPMS.Domain.Entities;

public class EmailConfigs
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string SmtpServerUrl { get; set; }
    public int PortNumber { get; set; }
    public string Username { get; set; }
    public string Password { get; set; }
    public bool SslRequired { get; set; }
    public bool IsActive { get; set; }
}