using System.ComponentModel.DataAnnotations;

namespace BPMS.Domain.Entities;

public class WorkFlowConfermentAuthorityDetail
{
    public WorkFlowConfermentAuthorityDetail()
    {
        Id = Guid.NewGuid();
    }
    [Key]
    public Guid Id { get; set; }

    public Guid ConfermentAuthorityId { get; set; }
    public Guid StaffId { get; set; }

    public int FromDate { get; set; }
    public int ToDate { get; set; }
    public bool OnlyOwnRequest { get; set; }

    //Navigation Property
    public virtual WorkFlowConfermentAuthority WorkFlowConfermentAuthority { get; set; }
    public virtual Staff Staffs { get; set; }

}