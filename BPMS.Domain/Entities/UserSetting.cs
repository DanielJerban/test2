namespace BPMS.Domain.Entities;

public class UserSetting
{
    public UserSetting()
    {
        Id = Guid.NewGuid();
    }

    public Guid Id { get; set; }
    public byte[]? Content { get; set; }

    public Guid UserId { get; set; }
    public virtual User User { get; set; }

    public Guid SettingTypeId { get; set; }
    public virtual LookUp SettingType { get; set; }
}