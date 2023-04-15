using BPMS.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace BPMS.Infrastructure.Data;

public class BpmsDbContext : DbContext
{
    public BpmsDbContext(DbContextOptions<BpmsDbContext> options) : base(options)
    {

    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(BpmsDbContext).Assembly);
        base.OnModelCreating(modelBuilder);
    }

    public DbSet<User> Users { get; set; }
    public DbSet<Staff> Staffs { get; set; }
    public DbSet<Role> Roles { get; set; }
    public DbSet<RoleAction> RoleActions { get; set; }
    public DbSet<RoleAccess> RoleAccesses { get; set; }
    public DbSet<Chart> Charts { get; set; }
    public DbSet<LookUp> LookUps { get; set; }
    public DbSet<OrganiztionInfo> OrganiztionInfos { get; set; }
    public DbSet<UserLoginOut> UserLoginOuts { get; set; }
    public DbSet<Schedule> Schedules { get; set; }
    public DbSet<Workflow> Workflows { get; set; }
    public DbSet<WorkFlowDetail> WorkFlowDetails { get; set; }
    public DbSet<WorkFlowNextStep> WorkFlowNextSteps { get; set; }
    public DbSet<Request> Requests { get; set; }
    public DbSet<Flow> Flows { get; set; }
    public DbSet<WorkFlowConfermentAuthority> WorkFlowConfermentAuthority { get; set; }
    public DbSet<WorkFlowConfermentAuthorityDetail> WorkFlowConfermentAuthorityDetails { get; set; }
    public DbSet<EmployementCertificate> EmployementCertificate { get; set; }
    public DbSet<WorkFlowForm> WorkFlowForms { get; set; }
    public DbSet<RoleMapChart> RoleMapCharts { get; set; }
    public DbSet<WorkflowEsb> WorkflowEsbs { get; set; }
    public DbSet<Company> Companies { get; set; }
    public DbSet<UserSetting> UserSettings { get; set; }
    public DbSet<Client> Clients { get; set; }
    public DbSet<WorkFlowFormList> WorkFlowFormList { get; set; }
    public DbSet<WorkFlowIndicator> WorkFlowIndicators { get; set; }
    public DbSet<ScheduleLog> ScheduleLogs { get; set; }
    public DbSet<Report> Reports { get; set; }
    public DbSet<DynamicChart> DynamicCharts { get; set; }
    public DbSet<Holyday> Holydays { get; set; }
    public DbSet<Assingnment> Assingnments { get; set; }
    public DbSet<FlowEvent> FlowEvents { get; set; }
    public DbSet<Exceptions> Exceptions { get; set; }
    public DbSet<FormClassification> FormClassifications { get; set; }
    public DbSet<FormClassificationRelation> FormClassificationRelations { get; set; }
    public DbSet<FormClassificationCreators> FormClassificationCreators { get; set; }
    public DbSet<FormClassificationAccess> FormClassificationAccess { get; set; }
    public DbSet<FormLookUp2N> FormLookUp2N { get; set; }
    public DbSet<ExternalApi> ExternalApis { get; set; }
    public DbSet<WorkFlowBoundary> WorkFlowBoundaries { get; set; }
    public DbSet<AllFlowsWithDelayLog> AllFlowsWithDelayLogs { get; set; }
    public DbSet<SmsProviderConfige> SmsProviderConfiges { get; set; }
    public DbSet<EmailConfigs> EmailConfigurations { get; set; }
    public DbSet<ServiceTaskLog> ServiceTaskLogs { get; set; }
    public DbSet<SmsLog> SmsLogs { get; set; }
    public DbSet<EmailLog> EmailLogs { get; set; }
    public DbSet<ThirdParty> ThirdParties { get; set; }
    public DbSet<RoleClaim> RoleClaims { get; set; }
    public DbSet<UserClaim> UserClaims { get; set; }
    public DbSet<UsefulLinks> UsefulLinks { get; set; }
    public DbSet<WorkflowDetailPatternItem> WorkflowDetailPatternItems { get; set; }
    public DbSet<WorkflowDetailPattern> WorkflowDetailPatterns { get; set; }
    public DbSet<RoleMapPostType> RoleMapPostTypes { get; set; }
    public DbSet<RoleMapPostTitle> RoleMapPostTitles { get; set; }
    public DbSet<SystemSetting> SystemSettings { get; set; }
    public DbSet<StartTimerEvent> StartTimerEvents { get; set; }
    public DbSet<AverageRequestProcessingTimeLog> AverageRequestProcessingTimeLogs { get; set; }
    public DbSet<DocumentsAccessType> DocumentsAccessTypes { get; set; }
    public DbSet<ScheduleLifeTimeLog> ScheduleLifeTimeLogs { get; set; }
}