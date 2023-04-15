namespace BPMS.Domain.Common.Dtos;

public class LdapSettingDto
{
    public bool IsLDAPUserSyncTimerEnable { get; set; }
    public string LDAPDomainName { get; set; }
    public string LDAPIp { get; set; }
    public string LDAPUserName { get; set; }
    public string LDAPPassword { get; set; }
}