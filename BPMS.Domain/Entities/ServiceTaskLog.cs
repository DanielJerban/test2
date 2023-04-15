namespace BPMS.Domain.Entities;

public class ServiceTaskLog
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string? WorkFlowTitle { get; set; }
    public string? WorkFlowVersion { get; set; }
    public string? WorkFlowDetailTitle { get; set; }
    public string? ServiceTaskObjName { get; set; }
    public int CreatedDate { get; set; }
    public string? CreatedTime { get; set; }
    public string? ExternalApiDataJson { get; set; }
}