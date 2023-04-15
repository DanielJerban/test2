using BPMS.Domain.Common.ViewModels;
using BPMS.Domain.Entities;
using BPMS.Infrastructure.Repositories.Base;

namespace BPMS.Infrastructure.Repositories;

public interface IWorkflowEsbRepository : IRepository<WorkflowEsb>
{
    //JsonResult FillDataForMessages(WorkflowEsbViewModel model);
    WorkflowEsb GetWorkflowEsbByWorkFlowNextStepIdAndEventId(Guid workFlowNextStepId, string eventId);
}