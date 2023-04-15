namespace BPMS.Domain.Common.Dtos.TncDataFix;

public class LoginDeserializeDataFixDto
{
    public LoginDeserialize Login { get; set; }
}

public class LoginDeserialize
{
    public bool State { get; set; }
    public string Token { get; set; }
}