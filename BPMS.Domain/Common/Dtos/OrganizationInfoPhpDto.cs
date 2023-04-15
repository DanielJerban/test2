namespace BPMS.Domain.Common.Dtos;

public class OrganizationInfoPhpDto
{
    public int PhpId { get; set; }
    public string PersonalCode { get; set; }
    public int? OrganizationPostId { get; set; }
    public int? ChartId { get; set; }
    public string IsActive { get; set; }
    public string Priority { get; set; }

    public string ApiKey { get; set; }
    public string Action { get; set; }
}