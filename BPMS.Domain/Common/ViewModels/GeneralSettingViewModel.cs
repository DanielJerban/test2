using System.ComponentModel.DataAnnotations;

namespace BPMS.Domain.Common.ViewModels;

public class GeneralSettingViewModel
{
    public string CreatorUserName { get; set; }
    public int CreateDate { get; set; }
    public string ActiveTab { get; set; }
    public string CreateTime { get; set; }
    public DateTime Date { get; set; }

    [Display(Name = "فعالسازی sync کاربران از طریق LDAP")]
    public bool IsLDAPUserSyncTimerEnable { get; set; }
        
    [Display(Name = "فعالسازی لاگین از طریق LDAP")]
    public bool IsLDAPLoginEnable { get; set; }
        
    [Display(Name = "دامنه")]
    public string LDAPDomainName { get; set; }
        
    [Display(Name = "IP")]
    public string LDAPIp { get; set; }
        
    [Display(Name = "نام کاربری")]
    public string LDAPUserName { get; set; }
        
    [Display(Name = "رمزعبور")]
    public string LDAPPassword { get; set; }
}