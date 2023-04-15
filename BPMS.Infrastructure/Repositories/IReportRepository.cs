using BPMS.Domain.Common.ViewModels;
using BPMS.Domain.Entities;
using BPMS.Domain.Common.Dtos;
using BPMS.Infrastructure.Repositories.Base;
using Microsoft.AspNetCore.Http;

namespace BPMS.Infrastructure.Repositories;

public interface IReportRepository : IRepository<Report>
{
    Guid SaveReport(ReportViewModel model, IFormFile file, string username, string webRootPath);
    IEnumerable<ReportViewModel> GetReportsForCurrentUser(Guid staffId);
    ReportViewModel LoadReport(Guid id);
    ReportViewModel GetReportById(Guid idValue);
    void DeleteReport(Guid id, string webRootPath);
    DynamicViewModel GenerateTreeForReportGenerator();
    string ExecuteQueryInDb(string code, List<string> jsonTables, List<List<string>> fields, ref int total, int pageNo = 0);
    List<ReportTreeViewModel> GetPropertiesFromProcess(Guid id);
    List<ReportTreeViewModel> GetPropertiesFromRequestType(Guid requestTypeId);
    string GetResultReportById(Guid id, ref int count, int pageNo = 0);
    DynamicReportViewModel GetReportByAccess(string username);
    IEnumerable<ReportViewModel> GetAllReport();
    List<RequestDelayGridViewModel> GetDelayedReports(Guid? chartId);
    List<RequestDelayGridViewModel> GetDelayedReport_AdminSubUsers(Guid staffId);
    List<RequestDelayGridViewModel> GetNoActionRequestReport();
    List<RequestDelayGridViewModel> GetAllRequestByAdminSubUsers(Guid staffId);
    IEnumerable<SelectListItem> GetReportByWorkflowId(Guid? workflowId, string username);
    IEnumerable<SelectListItem> GetWorkflowByRequestTypeId(Guid? requestTypeId, string username);
    List<StaffViewModel> GetManagers();
    List<RequestDelayViewModel> GetSpecificChartIncompletedRequests(Guid? chartId);
    void RemindUserTasksThrowAutomation();
    void SendFlowsReportThrowAutomation();
}