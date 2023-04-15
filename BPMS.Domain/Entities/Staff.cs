using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace BPMS.Domain.Entities;

public class Staff
{
    public Staff()
    {
        Users = new HashSet<User>();
        Id = Guid.NewGuid();
        OrganiztionInfos = new HashSet<OrganiztionInfo>();
        Workflows = new HashSet<Workflow>();
        WorkflowDetails = new HashSet<WorkFlowDetail>();
        Requests = new HashSet<Request>();
        FlowStaff = new HashSet<Flow>();
        ConfermentAuthorityFlow = new HashSet<Flow>();
        WorkFlowConfermentAuthorityDetail = new HashSet<WorkFlowConfermentAuthorityDetail>();
        Reports = new HashSet<Report>();
        WorkFlowForms = new HashSet<WorkFlowForm>();
        DynamicCharts = new HashSet<DynamicChart>();
        Assingnments = new HashSet<Assingnment>();
        WorkflowsModifier = new HashSet<Workflow>();
        WorkFlowFormsModifire = new HashSet<WorkFlowForm>();
    }

    [Key]
    public Guid Id { get; set; }

    [MaxLength(40, ErrorMessage = "{0} نباید بیشتر از 10 حرف باشد")]
    [Required(ErrorMessage = "{0} را وارد نمایید")]
    [Display(Name = "نام کاربری")]
    public string PersonalCode { get; set; }

    [Required(ErrorMessage = "{0} را وارد نمایید")]
    [Display(Name = "نام")]
    public string FName { get; set; }

    [Required(ErrorMessage = "{0} را وارد نمایید")]
    [Display(Name = "نام خانوادگی")]

    public string LName { get; set; }

    [Display(Name = "شماره موبایل")]
    public string? PhoneNumber { get; set; }

    [Display(Name = "آدرس ایمیل")]
    [DisplayName("آدرس ایمیل")]
    [DataType(DataType.EmailAddress)]
    public string? Email { get; set; }
    
    [Display(Name = "تصویر پروفایل")]
    [Required]
    public string ImagePath { get; set; }
    
    [Display(Name = "نوع کاربری")]
    [Required]
    public Guid StaffTypeId { get; set; }
    
    [Required]
    public Guid EngTypeId { get; set; }
    
    [MaxLength(10)]
    public string? LocalPhone { get; set; }
    public Guid? BuildingId { get; set; }
    public string? InsuranceNumber { get; set; }
    public string IdNumber { get; set; }
    public int? EmploymentDate { get; set; }
    public int? AgreementEndDate { get; set; }
    public byte Gender { get; set; }

    //Navigation Property
    public virtual LookUp StaffType { get; set; }
    public virtual LookUp EngType { get; set; }
    public virtual LookUp? Building { get; set; }

    public virtual ICollection<User> Users { get; set; }
    public virtual ICollection<OrganiztionInfo> OrganiztionInfos { get; set; }

    public virtual ICollection<Workflow> Workflows { get; set; }
    public virtual ICollection<Workflow> WorkflowsModifier { get; set; }
    public virtual ICollection<WorkFlowDetail> WorkflowDetails { get; set; }
    public virtual ICollection<Request> Requests { get; set; }
    public virtual ICollection<Flow> FlowStaff { get; set; }
    public virtual ICollection<Flow> ConfermentAuthorityFlow { get; set; }
    public virtual ICollection<WorkFlowConfermentAuthority> WorkFlowConfermentAuthority { get; set; }
    public virtual ICollection<WorkFlowConfermentAuthorityDetail> WorkFlowConfermentAuthorityDetail { get; set; }
    public virtual ICollection<Report> Reports { get; set; }
    public virtual ICollection<WorkFlowForm> WorkFlowForms { get; set; }
    public virtual ICollection<WorkFlowForm> WorkFlowFormsModifire { get; set; }
    public virtual ICollection<DynamicChart> DynamicCharts { get; set; }
    public virtual ICollection<Assingnment> Assingnments { get; set; }
    public virtual ICollection<FormClassificationCreators> FormClassificationCreators_Staff { get; set; }

    public string FullName => FName + " " + LName;
}