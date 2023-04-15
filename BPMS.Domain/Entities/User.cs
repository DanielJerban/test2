using System.ComponentModel.DataAnnotations;

namespace BPMS.Domain.Entities;

public class User
{
    public User()
    {
        RoleAccesses = new HashSet<RoleAccess>();
        UserLoginOuts = new HashSet<UserLoginOut>();
        UserSettings = new HashSet<UserSetting>();
        Id = Guid.NewGuid();
        SerialNumber = Guid.NewGuid();
    }
    [Key]
    public Guid Id { get; set; }

    [Required(ErrorMessage = "کاربری انتخاب نشده است")]
    public Guid StaffId { get; set; }

    [MaxLength(50, ErrorMessage = "{0} نباید بیشتر از 50 حرف باشد")]
    [Required(ErrorMessage = "{0} وارد نشده است")]
    [Display(Name = "نام کاربری")]
    public string UserName { get; set; }

    [Required(ErrorMessage = "{0} وارد نشده است")]
    [Display(Name = "کلمه عبور")]
    [DataType(DataType.Password)]
    public string Password { get; set; }
    [Display(Name = "وضعیت")]
    public bool IsActive { get; set; }

    public string? LDAPUserName { get; set; }
    public string? LDAPDomainName { get; set; }

    public Guid SerialNumber { get; set; }
    public bool TwoStepVerification { get; set; }
    public bool TwoStepVerificationByEmail { get; set; }
    public bool TwoStepVerificationByGoogleAuthenticator { get; set; }
    public string? GoogleAuthKey { get; set; }

    public virtual Staff Staff { get; set; }

    public virtual ICollection<RoleAccess> RoleAccesses { get; set; }

    public ICollection<UserClaim> UserClaims { get; set; }


    public virtual ICollection<UserLoginOut> UserLoginOuts { get; set; }
    public virtual ICollection<UserSetting> UserSettings { get; set; }
    public ICollection<SystemSetting> SystemSettings { get; set; }
}