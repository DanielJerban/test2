namespace BPMS.Domain.Common.Dtos;

public class UserDto
{
    public string UserName { get; set; }
    public Guid Id { get; set; }

    public string Password { get; set; }
    public bool RememberMe { get; set; }
    public string Ip { get; set; }
    public string MachineName { get; set; }
    public string Code { get; set; }
    public string SecurityKey { get; set; }
    public string Mobile { get; set; }

    public bool IsAutomationLogin { get; set; } = false;
    public string ApiKey { get; set; }
}

public class UserBaseInfoCacheDTO
{
    public string UserName { get; set; }
    public Guid Id { get; set; }
    public Guid StaffId { get; set; }
    public string FullName { get; set; }
}