using BPMS.Domain.Common.Enums;

namespace BPMS.Infrastructure.Services;

public interface IJobService
{
    void SendReminderMessage();
    void AutoAcceptAndReject();
    void ActivateInterruptingBoundaryTimer();
    void ActivateNonInterruptingBoundaryTimer(string webRootPath);
    void ActivateIntermediateTimerNotation();
    void CalculateAllRequestsDelay();
    void SubPersonnelDelayedRequests();
    void RunTimerStartEvent(string webRootPath);
    void CalculateAverageProcessingLog();
    void SyncLdapUsers();
    void ExecuteJob(ScheduleType scheduleType, string webRootPath);
}