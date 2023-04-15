namespace BPMS.Domain.Common.Dtos.FlowServiceDTOs;

public class GetUsersContainsInFlowStatusChangeOutputDTO
{
    public List<StaffFlowDTO> NoActionStaffs { get; set; }
    public StaffSimpleDTO RequesterStaff { get; set; }
    public long RequestNo { get; set; }
}