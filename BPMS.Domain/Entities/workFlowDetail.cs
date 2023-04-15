using System.ComponentModel.DataAnnotations;

namespace BPMS.Domain.Entities;

public class WorkFlowDetail
{
    public WorkFlowDetail()
    {
        Id = Guid.NewGuid();
        IsAdHoc = false;
        WorkFlowNextStepsFrom = new HashSet<WorkFlowNextStep>();
        WorkFlowNextStepsTo = new HashSet<WorkFlowNextStep>();
        Flows = new HashSet<Flow>();
        WorkFlowIndicators = new HashSet<WorkFlowIndicator>();
        WorkFlowBoundaries = new HashSet<WorkFlowBoundary>();
    }
    [Key]
    public Guid Id { get; set; }
    public Guid WorkFlowId { get; set; }
    [MaxLength(200)]
    [Required]
    public string Title { get; set; }
    public Guid? StaffId { get; set; }
    public Guid? OrganizationPostTypeId { get; set; }
    public Guid? OrganizationPostTitleId { get; set; }
    public Guid? WorkFlowFormId { get; set; }
    public Guid? ResponseGroupId { get; set; }
    public Guid? CallProcessId { get; set; }
    public Guid? WorkflowDetailPatternId { get; set; }

    public int? Step { get; set; }

    [MaxLength(200)]
    public string? ViewName { get; set; }
    public ExiteMethod? ExiteMethod { get; set; }
    public bool IsMultiConfirmReject { get; set; }
    public bool RequesterAccept { get; set; }
    public bool IsOrLogic { get; set; }
    public bool NoReject { get; set; }
    public bool BusinessAcceptor { get; set; }
    public bool SelectAcceptor { get; set; }
    public bool SelectFirstPostPattern { get; set; }
    public bool SelectAllPostPattern { get; set; }

    public int? WaitingTime { get; set; }

    public string? BusinessAcceptorMethod { get; set; }
    public string? ScriptTaskMethod { get; set; }
    public string? EditableFields { get; set; }
    public string? HiddenFields { get; set; } // key value json => if true the field will be hidden
    public int? WaithingTImeForAct { get; set; }
    [MaxLength(5)]
    public string? Act { get; set; }
    public string? PrintFileName { get; set; }

    public string Dsr { get; set; }
    public bool IsAdHoc { get; set; }

    public string? AdHocWorkflowDetails { get; set; }

    public bool IsServiceTask { get; set; }
    // Service task api response object name
    public string? ServiceTaskApiResponse { get; set; }
    public Guid? ExternalApiId { get; set; }

    public bool IsManualTask { get; set; }
    public bool IsScriptTask { get; set; }
    public string? Info { get; set; }
    public bool HasSaveableForm { get; set; }

    //Navigation Property
    public virtual ExternalApi? ExternalApi { get; set; }
    public virtual Staff? Staff { get; set; }
    public virtual LookUp? OrganizationPostType { get; set; }
    public virtual LookUp? OrganizationPostTitle { get; set; }
    public virtual LookUp? ResponseGroup { get; set; }
    public virtual WorkFlowForm? WorkFlowForm { get; set; }
    public virtual Workflow WorkFlow { get; set; }
    public WorkflowDetailPattern WorkflowDetailPattern { get; set; }
    //public virtual Workflow SubprocessWorkflow { get; set; }
    public virtual ICollection<WorkFlowNextStep> WorkFlowNextStepsFrom { get; set; }
    public virtual ICollection<WorkFlowNextStep> WorkFlowNextStepsTo { get; set; }
    public virtual ICollection<Flow> Flows { get; set; }
    public virtual ICollection<WorkFlowIndicator> WorkFlowIndicators { get; set; }
    public virtual ICollection<WorkFlowBoundary> WorkFlowBoundaries { get; set; }

}
public enum ExiteMethod
{

    [Display(Name = "مجتمع")]
    Integrated = 1,
    [Display(Name = "مستقل")]
    Standalone = 2

}