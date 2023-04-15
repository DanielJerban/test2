namespace BPMS.Domain.Common.Dtos;

public class ValidateAcceptorsDto
{
    public bool RequesterAccept { get; set; }
    public bool SelectAcceptor { get; set; }
    public bool BusinessAcceptor { get; set; }
    public string OrganizationPostTitleId { get; set; }
    public string OrganizationPostTypeId { get; set; }
    public string ResponseGroupId { get; set; }
    public string PatternId { get; set; }
    public bool SelectPatternFirst { get; set; }
    public bool SelectPatternAll { get; set; }
    public string StaffId { get; set; }
}