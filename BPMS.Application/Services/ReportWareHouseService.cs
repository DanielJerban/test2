using BPMS.Domain.Entities;
using BPMS.Infrastructure;
using BPMS.Infrastructure.Services;

namespace BPMS.Application.Services;

public class ReportWareHouseService : IReportWareHouseService
{
    private readonly IUnitOfWork _unitOfWork;

    public ReportWareHouseService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public void SetAverageRequestProcessingTime()
    {
        var insertResult = new List<AverageRequestProcessingTimeLog>();
        var queryRequests = _unitOfWork.AllFlowsWithDelayLogs.GetRequestUponWorkFlows();

        foreach (var reqWDto in queryRequests)
        {
            var thisWorkflowRequests = reqWDto.RequestWorkFlowDto.Select(i => i.Request).ToList();
            var data = _unitOfWork.AllFlowsWithDelayLogs.GetWorkflowDetailForReport(reqWDto.WorkflowId, thisWorkflowRequests);

            var totalActivity = data.TotalActivity.ToList();

            var min = 0.0;
            var max = 0.0;
            var ave = 0.0;
            double? aveWaitingTime = 0.0;

            var currentWorkflow = reqWDto.RequestWorkFlowDto.Select(rwdf => rwdf.Workflow).FirstOrDefault();
            if (currentWorkflow == null)
            {
                continue;
            }
            var currentRequestType = _unitOfWork.LookUps.SingleOrDefault(c => c.Id == currentWorkflow.RequestTypeId);
            var workFlowTitle = currentRequestType == null ? "" : currentRequestType.Title;
            var title = workFlowTitle + " / نسخه " + currentWorkflow.OrginalVersion + "." + currentWorkflow.SecondaryVersion;

            totalActivity.ForEach(i =>
            {
                ave += i.Avg;
                aveWaitingTime += i.WathingTime;
            });
            var count = thisWorkflowRequests.Count;
            if (totalActivity.Any())
            {
                max = totalActivity.Max(i => i.Avg);
                min = totalActivity.Min(i => i.Avg);
            }

            var processName = title;
            var aveWaiteTime = aveWaitingTime ?? 0.0;
            var avgTimeToDo = (int)(Math.Round(value: aveWaiteTime / count, digits: 0, mode: MidpointRounding.AwayFromZero));
            var avgTimeDone = (int)(Math.Round(value: ave / count, digits: 0, mode: MidpointRounding.AwayFromZero));
            var thisMin = (int)(Math.Round(value: min, digits: 0, mode: MidpointRounding.AwayFromZero));
            var thisMax = (int)(Math.Round(value: max, digits: 0, mode: MidpointRounding.AwayFromZero));
            var thisCount = count;
            var workFlowId = reqWDto.WorkflowId;

            var entity = _unitOfWork.AverageRequestProcessingTimeLog.SingleOrDefault(c => c.WorkFlowId == workFlowId);

            if (entity == null)
            {
                if (insertResult.Any(c => c.WorkFlowId == workFlowId))
                {
                    continue;
                }

                var averageRequest = AverageRequestProcessingTimeLog.CreatedNew(workFlowId, processName, avgTimeToDo, avgTimeDone, thisMin, thisMax, thisCount);
                insertResult.Add(averageRequest);
            }
            else
            {
                entity.UpdateEntity(workFlowId, processName, avgTimeToDo, avgTimeDone, thisMin, thisMax, thisCount);
                _unitOfWork.AverageRequestProcessingTimeLog.Update(entity);
            }
        }

        if (insertResult.Count > 0)
        {
            _unitOfWork.AverageRequestProcessingTimeLog.AddRange(insertResult);
        }

        _unitOfWork.Complete();
    }
}