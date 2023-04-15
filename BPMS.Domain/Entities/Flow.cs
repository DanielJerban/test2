using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BPMS.Domain.Entities;

public class Flow
{
    public Flow()
    {
        Id = Guid.NewGuid();
        IsActive = true;
        IsRead = false;
        FlowEvents = new HashSet<FlowEvent>();
    }
    [Key]
    public Guid Id { get; set; }

    public Guid StaffId { get; set; }
    public Guid? OrganizationPostTitleId { get; set; }

    public Guid RequestId { get; set; }

    public Guid FlowStatusId { get; set; }

    public int? ResponseDate { get; set; }

    [MaxLength(4)]

    public string? ResponseTime { get; set; }
    public int? DelayDate { get; set; }

    [MaxLength(4)]

    public string? DelayTime { get; set; }

    [DefaultValue(false)]
    public bool IsBalloon { get; set; }

    public string? Dsr { get; set; }

    public Guid WorkFlowDetailId { get; set; }

    public Guid? PreviousWorkFlowDetailId { get; set; }

    public Guid? PreviousFlowId { get; set; }

    public Guid? ConfermentAuthorityStaffId { get; set; }

    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public long Order { get; set; }

    public bool IsEnd { get; set; }
    public bool IsRead { get; set; }

        
    public bool IsActive { get; set; }
    public Guid? CallActivityId { get; set; }

    public int? DynamicWaitingTime { get; set; }
    public byte[]? Value { get; set; }

    //Navigation Property
    public virtual Staff Staff { get; set; }
    public virtual Request Request { get; set; }
    public virtual LookUp LookUpFlowStatus { get; set; }
    public virtual LookUp OrganizationPostTitle { get; set; }
    public virtual WorkFlowDetail WorkFlowDetail { get; set; }
    public virtual Flow? PreviousFlow { get; set; }
    public virtual Staff ConfermentAuthorityStaff { get; set; }
    public virtual ICollection<FlowEvent> FlowEvents { get; set; }
}