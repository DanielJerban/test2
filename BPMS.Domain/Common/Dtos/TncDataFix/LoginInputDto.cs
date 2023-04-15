namespace BPMS.Domain.Common.Dtos.TncDataFix;

public class LoginInputDto
{
    public Uri Url { get; set; }
    public string DatabaseName { get; set; }
    public string AdminUserName { get; set; }
    public string AdminPassword { get; set; }
}