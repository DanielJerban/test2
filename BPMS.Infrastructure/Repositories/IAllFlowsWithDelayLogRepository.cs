using BPMS.Domain.Common.ViewModels;
using BPMS.Domain.Common.ViewModels.Report;
using BPMS.Domain.Entities;
using Kendo.Mvc.UI;
using BPMS.Infrastructure.Repositories.Base;

namespace BPMS.Infrastructure.Repositories;

public interface IAllFlowsWithDelayLogRepository : IRepository<AllFlowsWithDelayLog>
{
    void UpdateTableLogSchedule();
    DataSourceResult GetAllLogs(DataSourceRequest request);
    DataSourceResult GetGeneralProcessStatus(DataSourceRequest request, string @from, string to);
    DataSourceResult GetLogsByChartId(DataSourceRequest request, Guid chartId);
    DataSourceResult GetLogsBySubUsers(DataSourceRequest request, Guid staffId, bool onlyDelay = false);
    DataSourceResult GetGeneralAllProcessStatus(DataSourceRequest request);
    List<GeneralProcessStatusViewModel> GetProcessStatusSelectedStep(Guid workflowId);
    List<RequestUponWorkflowDto> GetRequestUponWorkFlows();
    WorkflowIndicatorDetailViewModel GetDetailsOfWorkflow(Guid workflowId, List<Request> relatedRequests);
    ReportWorkflowIndicatorDetailViewModel GetWorkflowDetailForReport(Guid workflowId, List<Request> relatedRequests);
}