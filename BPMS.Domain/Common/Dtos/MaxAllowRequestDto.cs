namespace BPMS.Domain.Common.Dtos;

public class MaxAllowRequestDto
{
    public string Action { get; set; }
    public string Ip { get; set; }
    public int RequestCount { get; set; }
    public DateTime RequestDate { get; set; }
}