namespace BPMS.Domain.Common.Dtos;

public class WorkFlowFormDto
{
    public Guid Id { get; set; }
    public string PName { get; set; }
    public byte[] Content { get; set; }
    public int OrginalVersion { get; set; }
    public int SecondaryVersion { get; set; }
    public string Version => OrginalVersion.ToString() + "," + SecondaryVersion.ToString();
}