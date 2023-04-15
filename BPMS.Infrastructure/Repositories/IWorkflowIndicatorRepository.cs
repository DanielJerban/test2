using BPMS.Domain.Common.Dtos;
using BPMS.Domain.Common.ViewModels;
using BPMS.Domain.Entities;
using BPMS.Infrastructure.Repositories.Base;

namespace BPMS.Infrastructure.Repositories;

public interface IWorkflowIndicatorRepository : IRepository<WorkFlowIndicator>
{
    IEnumerable<WorkFlowIndicatorViewModel> GetWorkflowIndicatorRecords();
    dynamic GetActivitiesByRequestType(Guid id);
    IEnumerable<LookUpViewModel> GetRequestTypeByActivityId(Guid? id, string currentUserName);
    IEnumerable<WorkflowIndicatorWidgetViewModel> GetWorkflowIndicatorRecordsForReport();
    WorkflowIndicatorDetailViewModel GetDetailsOfWorkflow(Guid workflowIndicatorId);
    void CreateWorkflowIndicator(WorkFlowIndicatorViewModel model);
    WorkflowIndicatorWidgetViewModel GetByWidgetId(Guid id);
    void DeleteIndicator(Guid id);
    WorkFlowRequestsDetailByRequestTypeDto GetDetailsOfWorkFlowByRequestTypeId(Guid requestTypeId);
    IEnumerable<LookUpViewModel> GetRequestTypeByPolicy(string username);
}