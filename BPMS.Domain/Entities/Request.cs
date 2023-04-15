using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BPMS.Domain.Entities;

public class Request
{
    public Request()
    {
        Id = Guid.NewGuid();
        Flows = new HashSet<Flow>();
        EmpCertificates = new HashSet<EmployementCertificate>();
    }

    [Key]
    public Guid Id { get; set; }
    public int RegisterDate { get; set; }

    [MaxLength(4)]
    public string? RegisterTime { get; set; }

    [DatabaseGenerated(databaseGeneratedOption: DatabaseGeneratedOption.Identity)]
    public long RequestNo { get; set; }

    public Guid WorkFlowId { get; set; }
    public Guid RequestStatusId { get; set; }
    public Guid StaffId { get; set; }
    public Guid OrganizationPostTitleId { get; set; }
    // Content of teh form goes here => JsonValue in front
    public byte[]? Value { get; set; }

    public bool IgnoreOrgInfChange { get; set; }

    //Navigation Property
    public virtual Staff Staff { get; set; }
    public virtual Workflow Workflow { get; set; }
    public virtual LookUp RequestStatus { get; set; }
    public virtual LookUp OrganizationPostTitle { get; set; }
    public virtual ICollection<Flow> Flows { get; set; }
    public virtual ICollection<EmployementCertificate> EmpCertificates { get; set; }
}