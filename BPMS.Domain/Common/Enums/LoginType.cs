using System.ComponentModel.DataAnnotations;

namespace BPMS.Domain.Common.Enums;

public enum LoginType
{
    [Display(Name = "لاگین از طریق نام کاربری و پسورد سیستم")]
    LoginWithSystem = 1,
    [Display(Name = "لاگین از طریق Ldap")]
    LoginWithLdap = 2,
    [Display(Name = "لاگین همزمان از طریق سیستم و Ldap")]
    LoginWithSystemAndLdap = 3
}