namespace BPMS.Domain.Common.ViewModels;

public class FlowParam
{
    public Guid? CurrentStep { get; set; }
    public Guid? CurrentFlowId { get; set; }
    public Guid ConfirmStaffId { get; set; }
    public Guid ConfirmOrganizationPostTitleId { get; set; }
    public byte? BsNextStep { get; set; }
    public IEnumerable<Guid> BsNextStaffIds { get; set; }
    public IEnumerable<Guid> SelectedStaffIds { get; set; }
    public string Path { get; set; }
    public Guid RequestTypeId { get; set; }
    public Guid StaffId { get; set; }
    public Guid OrganizationPostTitleId { get; set; }
    public Guid RequestId { get; set; }
    public string Evt { get; set; }
    public bool IsEnd { get; set; }
    public string Dsr { get; set; }
    public Guid? ApiStaffId { get; set; }
    public dynamic Work { get; set; }
    public Guid WorkFlowDetailId { get; set; }
    public string BoundaryName { get; set; }
    public bool IsAdHok { get; set; }
}