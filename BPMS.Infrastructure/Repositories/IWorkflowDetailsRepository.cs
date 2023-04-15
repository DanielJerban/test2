using BPMS.Domain.Common.Dtos;
using BPMS.Domain.Common.ViewModels;
using BPMS.Domain.Entities;
using BPMS.Infrastructure.Repositories.Base;
using Microsoft.AspNetCore.Http;

namespace BPMS.Infrastructure.Repositories;

public interface IWorkflowDetailsRepository : IRepository<WorkFlowDetail>
{
    WorkFlowDetail GetWithWorkFlow(Guid workflowId, int i);
    WorkFlowDetail GetWithRequestType(Guid requestTypeId, Guid i);
    WorkFlowDetail GetWithRequestType(Guid requestTypeId, int i);
    WorkFlowViewModel ShowDiagram(Guid workflowDetailId, Guid requestId);
    IEnumerable<WorkFlowDetailViewModel> GetActiveWorkFlowDetailsByRequestType(Guid empty);
    IEnumerable<WorkFlowDetailViewModel> GetWorkflowDetail(Guid workflowDetailId, int code);
    void UploadStimullFile(IFormFile? file, string filePath, string webRootPath);
    IEnumerable<DynamicViewModel> GetAllStimulInBpmn(string webRootPath);
    WorkFlowDetail GetWithRequestId(Guid requestId, int i);
    IEnumerable<WorkFlowDetailViewModel> GetWorkflowDetainForAdHoc(Guid id);
    IEnumerable<SelectListItem> GetAdHocWorkflowDetails(Request request, Workflow workflowBase);
    WorkFlowDetail FindWorkFlowDetailById(Guid workFlowDetailId);
    WorkFlowDetail GetWorkFlow(Guid? workFlowDetailId);
}