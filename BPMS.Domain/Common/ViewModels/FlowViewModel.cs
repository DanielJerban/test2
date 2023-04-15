using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace BPMS.Domain.Common.ViewModels;

public class FlowViewModel
{
    public Guid Id { get; set; }
    public Guid StaffId { get; set; }
    public Guid RequestId { get; set; }
    public Guid FlowStatusId { get; set; }
    public int? ResponseDate { get; set; }

    [MaxLength(4)]
    public string ResponseTime { get; set; }

    [DefaultValue(false)]
    public bool IsBalloon { get; set; }

    public string Dsr { get; set; }
    public Guid WorkFlowDetailId { get; set; }
    public Guid? PreviousFlowId { get; set; }
    public Guid ConfermentAuthorityStaffId { get; set; }
}