using System.ComponentModel.DataAnnotations;

namespace BPMS.Domain.Entities;

public  class Workflow
{
    public Workflow()
    {
        Id = Guid.NewGuid();
        WorkflowDetails = new HashSet<WorkFlowDetail>();
        Requests=new HashSet<Request>();
        Reports=new HashSet<Report>();
    }
    [Key]
    public Guid Id { get; set; }
    public Guid RequestTypeId { get; set; }
    public Guid? RequestGroupTypeId { get; set; }
       
    public string? RemoteId { get; set; }
    public string? CodeId { get; set; }
    public Guid StaffId { get; set; }
    public int RegisterDate { get; set; }
    [MaxLength(4)]
    public string? RegisterTime { get; set; }
    public Guid? ModifiedId { get; set; }
    public int? ModifideDate { get; set; }
    [MaxLength(4)]
    public string? ModifideTime { get; set; }
    [Display(Name = "فعال")]
    public bool IsActive { get; set; }
    [Display(Name = "نسخه اصلی")]
    [Required(ErrorMessage = "{0} را وارد نمایید.")]
    public int OrginalVersion { get; set; }
    [Display(Name = "نسخه فرعی")]
    [Required(ErrorMessage = "{0} را وارد نمایید.")]
    public int SecondaryVersion { get; set; }
    [Display(Name = "توضیحات")]
    public string? Dsr { get; set; }
    [Display(Name = "شرح فرآیند")]
    public string? About { get; set; }
    public string? KeyWords { get; set; }
    public byte[]? Content { get; set; }
    public Guid FlowTypeId { get; set; }
    public string? Owner { get; set; }
    public Guid? SubProcessId { get; set; }
    public Guid? ExternalId { get; set; }

    //Navigation Property
    public virtual LookUp RequestType { get; set; }
    public virtual LookUp FlowType { get; set; }
    public virtual LookUp? RequestGroupType { get; set; }
    public virtual Staff Staff { get; set; }
    public virtual Staff? Modifier { get; set; }
    public virtual Workflow? SubProcess { get; set; }
    public virtual ICollection<Workflow> SubProcesses { get; set; }
    public virtual ICollection<WorkFlowDetail> WorkflowDetails { get; set; }
    public virtual ICollection<Request> Requests { get; set; }
    public virtual ICollection<Report> Reports { get; set; }
    public virtual StartTimerEvent? StartTimerEvent { get; set; }
}