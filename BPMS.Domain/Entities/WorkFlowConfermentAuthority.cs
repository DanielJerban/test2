using System.ComponentModel.DataAnnotations;

namespace BPMS.Domain.Entities;

public class WorkFlowConfermentAuthority
{
    public WorkFlowConfermentAuthority()
    {
        Id = Guid.NewGuid();
        WorkFlowConfermentAuthorityDetail = new HashSet<WorkFlowConfermentAuthorityDetail>();
    }
    [Key]
    public Guid Id { get; set; }
    public Guid RequestTypeId { get; set; }
    public Guid StaffId { get; set; }
    [Display(Name = "تاریخ ثبت")]
    public int RegisterDate { get; set; }
    //Navigation Property
    public virtual LookUp LookUpRequestType { get; set; }
    public virtual Staff Staff { get; set; }
    public virtual ICollection<WorkFlowConfermentAuthorityDetail> WorkFlowConfermentAuthorityDetail { get; set; }
}