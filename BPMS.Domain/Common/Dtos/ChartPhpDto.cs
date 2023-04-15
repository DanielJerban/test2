namespace BPMS.Domain.Common.Dtos;

public class ChartPhpDto
{

    public int PhpId { get; set; }
    public string Title { get; set; }
    public string ChartLevelId { get; set; }
    public string ParentId { get; set; }
    public string IsActive { get; set; }

    public string ApiKey { get; set; }
    public string Action { get; set; }
}