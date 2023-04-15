namespace BPMS.Domain.Common.Dtos;

public class OrganizationInfoDto
{
    public Guid Id { get; set; }

    public Guid StaffId { get; set; }
    public string Staff { get; set; }
    public Guid OrganiztionPostId { get; set; }
    public string OrganiztionPost { get; set; }
    public Guid ChartId { get; set; }
    public string Chart { get; set; }

    public bool Priority { get; set; }
    public bool IsActive { get; set; }


}