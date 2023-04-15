using BPMS.Domain.Common.Enums;
using System.ComponentModel.DataAnnotations;

namespace BPMS.Domain.Common.ViewModels;

public class LdapSettingViewModel
{
    [Display(Name = "فعالسازی sync کاربران از طریق LDAP")]
    public bool IsSyncerEnable { get; set; }

    [Display(Name = "فعالسازی لاگین از طریق")]
    public LoginType LoginType { get; set; }

    [Display(Name = "دامنه")]
    public string DomainName { get; set; }

    [Display(Name = "IP")]
    public string Ip { get; set; }

    [Display(Name = "نام کاربری")]
    public string UserName { get; set; }

    [Display(Name = "رمزعبور")]
    public string Password { get; set; }
    public LdapMapperViewModel Properties { get; set; }
}