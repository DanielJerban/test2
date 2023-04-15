namespace BPMS.Domain.Common.Dtos;

public class ChartDto
{
    public Guid Id { get; set; }
    public string Title { get; set; }
    public Guid ChartLevelId { get; set; }
    public string ChartLevel { get; set; }
    public Guid? ParentId { get; set; }
    public bool IsActive { get; set; }

}