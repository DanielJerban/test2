namespace BPMS.Domain.Common.Dtos;

public class CreateworkDto
{
    public string RemoteId { get; set; }
    public string Content { get; set; }
    public string StaffSelected { get; set; }
    public string GridList { get; set; }
    public Guid OrganizationPostTitleId { get; set; }
}