namespace BPMS.Domain.Common.Dtos;

public class ProcessDto
{
    //  public string ApiKey { get; set; }
    public Guid? OrganizationPostTitleId { get; set; }
    public Guid RequestTypeId { get; set; }
    public Guid StaffId { get; set; }
    public Guid FlowId { get; set; }
    public Guid RequestId { get; set; }
    public int Code { get; set; }

    public string ReturnUrl { get; set; }
}