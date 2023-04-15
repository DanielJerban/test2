namespace BPMS.Domain.Common.Dtos.FlowServiceDTOs;

public class StaffFlowDTO
{
    public Guid StaffId { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string PersonalCode { get; set; }

    public Guid FlowId { get; set; }

    public Guid WorkFlowDetailId { get; set; }
    public string WorkFlowDetailTitle { get; set; }
}