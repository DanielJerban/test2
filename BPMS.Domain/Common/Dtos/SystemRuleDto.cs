namespace BPMS.Domain.Common.Dtos;

public class SystemRuleDto
{
    public SystemRuleDto()
    {
        Inputs = new List<object>();
    }
    public string MemberName { get; set; }
    public string Operator { get; set; }
    public string TargetValue { get; set; }
    public List<SystemRuleDto> Rules { get; set; }
    public List<object> Inputs { get; set; }
}