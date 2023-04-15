using BPMS.Infrastructure.Repositories;

namespace BPMS.Infrastructure;

public interface IUnitOfWork : IDisposable
{
    IChartRepository Charts { get; }
    ILookUpRepository LookUps { get; }
    IOrganizationInfoRepository OrganizationInfos { get; }
    IRoleRepository Roles { get; }
    IRoleAccessRepository RoleAccesses { get; }
    IRoleActionRepository RoleActions { get; }
    IScheduleRepository Schedules { get; }
    IStaffsRepository Staffs { get; }
    IUserRepository Users { get; }
    IWorkflowRepository Workflows { get; }
    IWorkflowDetailsRepository WorkflowDetails { get; }
    IWorkflowNextStepsRepository WorkflowNextSteps { get; }
    IFlowRepository Flows { get; }
    IFlowEventRepository FlowEvents { get; }
    IRequestRepository Request { get; }
    IWorkFlowFormRepository WorkFlowForm { get; }

    IWorkFlowConfermentAuthorityRepository WorkFlowConfermentAuthority { get; }
    IWorkFlowConfermentAuthorityDetailRepository WorkFlowConfermentAuthorityDetail { get; }
    IEmployementCertificateRepository EmployementCertificate { get; }
    IRoleMapChartRepository RoleMapChart { get; }
    IWorkflowEsbRepository WorkflowEsb { get; }
    ICompanyRepository Company { get; }
    IClientRepository Client { get; }

    IUserSettingRepository UserSetting { get; }
    IWorkFlowFormListRepository WorkFlowFormLists { get; }
    IScheduleLogRepository ScheduleLogs { get; }
    IReportRepository Reports { get; }
    IWorkflowIndicatorRepository WorkflowIndicators { get; }
    IDynamicChartRepository DynamicCharts { get; }
    IHolydayRepository Holydays { get; }
    IAssingnmentRepository Assingnments { get; }
    IExceptionsRepository Exceptions { get; }
    IFormClassificationRepository FormClassifications { get; }
    IFormClassificationCreatorsRepository FormClassificationCreators { get; }
    IFormClassificationAccessRepository FormClassificationAccess { get; }
    IFormLookUp2NRepository FormLookUp2N { get; }
    IExternalApiRepository ExternalApis { get; }
    IWorkFlowBoundaryRepository WokFlowBoundaris { get; }
    IAllFlowsWithDelayLogRepository AllFlowsWithDelayLogs { get; }
    IFormClassificationRelationRepository FormClassificationRelation { get; }
    IEmailConfigsRepository EmailConfigurations { get; }
    ISmsLogRepository SmsLogRepository { get; }
    IEmailLogRepository EmailLogRepository { get; }
    IJiraLogRepository JiraLogRepository { get; }
    IThirdPartyRepository ThirdPartyRepository { get; }
    IUserClaimRepository UserClaim { get; }
    IRoleClaimRepository RoleClaim { get; }
    IUsefulLinksRepository UsefulLinks { get; }
    IWorkFlowDetailPatternRepository WorkFlowDetailPattern { get; }
    IWorkFlowDetailPatternItemRepository WorkFlowDetailPatternItem { get; }
    ISystemSettingRepository SystemSetting { get; }
    IAverageRequestProcessingTimeLogRepository AverageRequestProcessingTimeLog { get; }
    IServiceTaskLogRepository ServiceTaskLog { get; }
    IDocumentsAccessTypeRepository DocumentsAccessType { get; }
    IScheduleLifeTimeLogRepository ScheduleLifeTimeLog { get; }

    int Complete();
}
