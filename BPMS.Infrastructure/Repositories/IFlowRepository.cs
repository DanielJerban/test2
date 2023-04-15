using BPMS.Domain.Common.Dtos;
using BPMS.Domain.Common.Dtos.FlowServiceDTOs;
using BPMS.Domain.Common.ViewModels;
using BPMS.Domain.Entities;
using Kendo.Mvc.UI;
using BPMS.Infrastructure.Repositories.Base;

namespace BPMS.Infrastructure.Repositories;

public interface IFlowRepository : IRepository<Flow>
{
    IEnumerable<FlowViewModel> GetAllCartbotRecords(Guid? id,string username);
    dynamic GetValueForStaticForms(Flow flow);
    void ChangeBalloonStatus(Request flowIds);
    void ChangeIsBalloonStatusInFlow(List<Guid> flowIds);
    WorkFlowFormViewModel ExternalCreateProcess(ProcessDto model);
    object GetFlowsByStaffId(Guid staffId, int i);
    void ChangeFlowsStaff(List<FlowViewModel> model, string oldStaffId);
    DataSourceResult GetFlows(Guid userStaffId, DataSourceRequest request, int code);
    void CheckEventGateWay(FlowEvent flowsEvent);

    string GetRequestTypeTitle(Guid workflowId, Guid? subprocessId, string restOfTitle);

    GetUsersContainsInFlowStatusChangeOutputDTO GetUsersContainsInFlowStatusChange(Guid requestId);
    void SetDynamicWaitingTime(Guid? flowId, dynamic work);
    Flow GetFlowById(Guid flowId);
    List<Guid> GetTimerStartEvents();
    void SetTimerLastRunDate(Guid id);
    double CalculateDelay(DateTime reqDate, DateTime compareTime, LookUp saturday2Wendsday, LookUp thursday, List<Holyday> holidays);

    double CalculateDoneTime(DateTime reqDate, DateTime compareTime, LookUp saturday2Wendsday, LookUp thursday,
        List<Holyday> holidays);

    DateTime? CalculateTimeToDo(DateTime reqDate, int? waitingTime, LookUp saturday2Wendsday, LookUp thursday, List<Holyday> holidays);
    IEnumerable<ThirdChartViewModel> CalculateSediment(int a, List<Holyday> holidays);
    Flow GetFlowByRequestIdAndWorkFlowDetailId(Guid requestId, Guid workflowDetailId);
    Flow ChangeFlowStatus(FlowParam param, Guid statusId);
    List<Flow> GetFlowList(Guid requestId, Guid? flowId, Guid workflowDetailId);
    IQueryable<string> GetRejectorPersonalCode(Guid staffId, Guid requestId);
    Guid? NextFlowIdIfAcepptorExist(Guid staffId);
    List<WorkFlowDetail> GetAllWorkFlowNextSteps(Guid wfdId);
    bool CheckInclusivePath(WorkFlowDetail workFlowD, FlowParam param, string gatewayName);
    void SendNotification(Guid requestId, MainFlowStaff nextflowstaffresult);
    List<WorkFlowBoundary> GetWorkflowBoundaries(Guid workflowId);

    List<FlowAndWorkFlowDetailViewModel>
        GetFlowAndWorkFlowDetailByRequestIdAndStaffId(Guid requestId, Guid staffId);

    List<FlowAndWorkFlowDetailViewModel> GetFlowAndWorkFlowDetailByRequestIdAndStaffIdForManualTask(Guid requestId,
        Guid staffId);

    List<FlowAndWorkFlowDetailViewModel> GetFlowAndWorkFlowDetailByRequestIdAndStaffIdForServiceTask(
        Guid requestId, Guid staffId);

    List<FlowAndWorkFlowDetailAndBoudaryViewModel> GetFlowAndWorkFlowDetailAndBoudary();
    List<FlowAndWorkFlowDetailAndBoudaryViewModel> GetByContainNonInterruptingBoundaryTimerEvent();
    Workflow GetWorkFlowIncludedTypeByIB(Guid Id);
    List<WorkFlowNextStep> GetWorkFlowNextSteps(string path, string boundaryId, Guid formWorkflowDetailId);
    List<WorkFlowNextStep> GetWorkFlowNextStepsWithoutBoundaryName(string path, Guid formWorkflowDetailId);
    void FindParentFromChartId(Guid? chartId, List<Chart> chartList);
    IQueryable<Workflow> GetWorkFlows();
    IQueryable<WorkFlowNextStep> GetWorkFlowNextSteps();
    IQueryable<Staff> GetStaffs();
    IQueryable<Assingnment> GetAssingnments();
    IQueryable<LookUp> GetLookUps();
    IQueryable<OrganiztionInfo> GetOrganiztionInfos();
}