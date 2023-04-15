using BPMS.Domain.Common.Enums;

namespace BPMS.Domain.Entities;

public class SystemSetting
{
    public SystemSetting()
    {
        Id = Guid.NewGuid();
        CreatedDate = DateTime.Now;
    }

    public Guid Id { get; set; }
    public DateTime CreatedDate { get; set; }
    public string? Data { get; set; }
    public SystemSettingType Type { get; set; }

    public Guid CreatorUserId { get; set; }
    public User CreatorUser { get; set; }
}