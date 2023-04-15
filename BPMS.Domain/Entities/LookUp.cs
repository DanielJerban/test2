using System.ComponentModel.DataAnnotations;

namespace BPMS.Domain.Entities;

public class LookUp
{
    public LookUp()
    {
        Id = Guid.NewGuid();
        Charts = new HashSet<Chart>();
        UserLoginOuts = new HashSet<UserLoginOut>();
        OrganiztionInfos = new HashSet<OrganiztionInfo>();
        Schedules = new HashSet<Schedule>();
        WorkflowsRequestType = new HashSet<Workflow>();
        WorkflowsFlowType = new HashSet<Workflow>();
        WorkflowsRequestGroupType = new HashSet<Workflow>();
        StaffType = new HashSet<Staff>();
        WorkFlowDetailOrganizationPostTitles = new HashSet<WorkFlowDetail>();
        WorkFlowDetailOrganizationPostTypes = new HashSet<WorkFlowDetail>();
        WorkFlowDetailResponseGroups = new HashSet<WorkFlowDetail>();
        RequestStatuses = new HashSet<Request>();
        RequestPostTitles = new HashSet<Request>();
        FlowsStatus = new HashSet<Flow>();
        FlowsOrganizationPostTitle = new HashSet<Flow>();
        WorkFlowConfermentAuthorities = new HashSet<WorkFlowConfermentAuthority>();
        EngType = new HashSet<Staff>();
        UserSettings = new HashSet<UserSetting>();
        Buildings = new HashSet<Staff>();
        WorkFlowIndicatorRequestType = new HashSet<WorkFlowIndicator>();
        WorkFlowIndicatorDuration = new HashSet<WorkFlowIndicator>();
        WorkFlowIndicatorFlowstatus = new HashSet<WorkFlowIndicator>();
        WorkFlowIndicatorCalcCriterion = new HashSet<WorkFlowIndicator>();
        WorkFlowIndicatorWidgetType = new HashSet<WorkFlowIndicator>();
        DynamicChartsWidgetType = new HashSet<DynamicChart>();
        HolydayTypes = new HashSet<Holyday>();
        Assingnments = new HashSet<Assingnment>();
        ExternalApiContentTypes = new HashSet<ExternalApi>();
        ExternalApiActions = new HashSet<ExternalApi>();
        RoleMapPostTypes = new HashSet<RoleMapPostType>();
        RoleMapPostTitles = new HashSet<RoleMapPostTitle>();
    }

    [Key]
    public Guid Id { get; set; }
    [Display(Name = "کد")]
    public int Code { get; set; }

    [Display(Name = "دسته بندی")]
    [MaxLength(500)]
    public string? Type { get; set; }

    [MaxLength(1000)]
    [Display(Name = "عنوان")]
    public string? Title { get; set; }

    [MaxLength(1000)]
    [Display(Name = "وابستگی اول")]
    public string? Aux { get; set; }

    [MaxLength(1000)]
    [Display(Name = "وابستگی دوم")]
    public string? Aux2 { get; set; }

    [Display(Name = "فعال/غیرفعال")]
    public bool IsActive { get; set; }


    //Navigation Property
    public virtual ICollection<Chart> Charts { get; set; }
    public virtual ICollection<OrganiztionInfo> OrganiztionInfos { get; set; }

    public virtual ICollection<UserLoginOut> UserLoginOuts { get; set; }
    public virtual ICollection<Schedule> Schedules { get; set; }

    public virtual ICollection<Workflow> WorkflowsRequestType { get; set; }
    public virtual ICollection<Workflow> WorkflowsFlowType { get; set; }
    public virtual ICollection<Workflow> WorkflowsRequestGroupType { get; set; }
    public virtual ICollection<WorkFlowDetail> WorkFlowDetailOrganizationPostTypes { get; set; }
    public virtual ICollection<WorkFlowDetail> WorkFlowDetailOrganizationPostTitles { get; set; }
    public virtual ICollection<WorkFlowDetail> WorkFlowDetailResponseGroups { get; set; }
    public virtual ICollection<Request> RequestStatuses { get; set; }
    public virtual ICollection<Request> RequestPostTitles { get; set; }
    public virtual ICollection<Flow> FlowsStatus { get; set; }
    public virtual ICollection<Flow> FlowsOrganizationPostTitle { get; set; }
    public virtual ICollection<WorkFlowConfermentAuthority> WorkFlowConfermentAuthorities { get; set; }
    public virtual ICollection<Staff> StaffType { get; set; }
    public virtual ICollection<Staff> EngType { get; set; }
    public virtual ICollection<Staff> Buildings { get; set; }

    public virtual ICollection<UserSetting> UserSettings { get; set; }
    public virtual ICollection<WorkFlowIndicator> WorkFlowIndicatorRequestType { get; set; }
    public virtual ICollection<WorkFlowIndicator> WorkFlowIndicatorDuration { get; set; }
    public virtual ICollection<WorkFlowIndicator> WorkFlowIndicatorFlowstatus { get; set; }
    public virtual ICollection<WorkFlowIndicator> WorkFlowIndicatorCalcCriterion { get; set; }
    public virtual ICollection<WorkFlowIndicator> WorkFlowIndicatorWidgetType { get; set; }
    public virtual ICollection<DynamicChart> DynamicChartsWidgetType { get; set; }
    public virtual ICollection<Holyday> HolydayTypes { get; set; }
    public virtual ICollection<Assingnment> Assingnments { get; set; }

    public virtual ICollection<FormClassification> FormClassification_FormType { get; set; }
    public virtual ICollection<FormClassification> FormClassification_FormStatus { get; set; }
    public virtual ICollection<FormClassification> FormClassification_StandardType { get; set; }
    public virtual ICollection<FormClassification> FormClassification_AccessType { get; set; }
    public virtual ICollection<FormClassificationCreators> FormClassificationCreators_CreatorType { get; set; }
    public virtual ICollection<FormClassification> FormClassification_WorkFlowLookup { set; get; }
    public virtual ICollection<FormClassification> FormClassification_ConfidentialLevel { set; get; }

    public virtual ICollection<ExternalApi> ExternalApiContentTypes { get; set; }
    public virtual ICollection<ExternalApi> ExternalApiActions { get; set; }
    public ICollection<WorkflowDetailPatternItem> WorkflowDetailPatternItems { get; set; }
    public virtual ICollection<RoleMapPostType> RoleMapPostTypes { get; set; }
    public virtual ICollection<RoleMapPostTitle> RoleMapPostTitles { get; set; }
}