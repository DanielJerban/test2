using BPMS.Domain.Common.Dtos;
using BPMS.Domain.Common.ViewModels;
using BPMS.Domain.Entities;

namespace BPMS.Infrastructure.Services;

public interface IFlowService
{
    void SendMessageForRejectRequest(FlowParam param, Request currentRequest);
    void ChangeRequestStatus(Guid requestId, Guid statusId, FlowParam param, Guid? callprocessId);
    SelectAcceptorViewModel RejectFlow(FlowParam param);
    SelectAcceptorViewModel AcceptFlow(FlowParam param);
    void AcceptRejectSimilarCode(WorkFlowDetail workflowDetail, FlowParam param, MainFlowStaff nextflowstaffresult);
    void DetectedRequestStatus(FlowParam param, MainFlowStaff nextflowstaffresult, List<Flow> nextFlows,
        List<Flow> flowNotProgress, Guid? callprocessId);
    void GotoNextStepInCallProcess(FlowParam param);
    void HandleEndEvent(List<WorkflowEsb> nextflowstaffresult, FlowParam param);
    void AutoAcceptScriptTaskFlow(Guid requestId);
    void AutoAcceptOrRejectManualTaskFlow(Guid requestId);
    void AutoAcceptServiceTaskFlow(Guid requestId, string webRootPath);
    void CheckWorkFLowBoundaryForSchedule();
    void CheckWorkFLowNonInterruptingBoundaryForSchedule(string webRootPath);
    SelectAcceptorViewModel CreateWork(FlowParam param);
    MainFlowStaff GetNextFlowStaff(FlowParam param);
    SelectAcceptorViewModel ApiCreateWork(CreateworkDto model, string username);
    void SendSmsAndEmail(EsbViewModel esb, dynamic paramWork);
    void SendSignalMessage(SignalViewModel item);
    void NextEvent(FlowEvent flowsEvent, List<FlowEvent> events, FlowParam param);
    List<Flow> FlowStaffToFlow(FlowStaff flowStaff, Guid? previousFlowid = null, Guid? previousWfdId = null);
    void CheckIntermediateTimerForSchedule();
    bool SaveWork(FlowParam param);

}