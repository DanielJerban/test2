namespace BPMS.Domain.Common.Dtos;

public class UserLoginOutDto
{
    public UserLoginOutDto()
    {
        Id = Guid.NewGuid();
    }
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public Guid LoginOutTypeId { get; set; }
    public int Date { get; set; }
    public string Time { get; set; }
    public string Ip { get; set; }
    public string MachineName { get; set; }
    public string BrowserName { get; set; }
    public string UserName { get; set; }
}