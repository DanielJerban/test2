using System.ComponentModel.DataAnnotations;

namespace BPMS.Domain.Entities;

public class ThirdParty
{
    [Key] 
    public Guid Id { get; set; } = Guid.NewGuid();

    public string? Name { get; set; }
    public string? Description { get; set; }
    public bool IsActive { get; set; }
    public string? UserName { get; set; }
    public string? Password { get; set; }
    public bool PasswordExpires { get; set; }
    public int? ExpireDate { get; set; }
    public string? IPAddresses { get; set; }
}