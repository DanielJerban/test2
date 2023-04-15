using BPMS.Domain.Common.ViewModels;
using BPMS.Domain.Entities;
using BPMS.Infrastructure.Repositories.Base;
using System.Xml.Linq;
using BPMS.Domain.Common.Dtos;

namespace BPMS.Infrastructure.Repositories;

public interface IWorkflowRepository : IRepository<Workflow>
{
    IEnumerable<RequestTypeDropDownViewModel> RequestTypesByPolicy(string username);
    IEnumerable<RequestTypeDropDownViewModel> RequestTypes(Guid staffId, bool remote = false);
    IEnumerable<SelectListItem> OrganizationPostTitle(Guid staffId);
    List<int> CountOfEachTypeByUserName(string username);
    IEnumerable<WorkFlowViewModel> GetAllWorkflows();
    List<int> CountOfEachType(string personelCode);
    BpmnDiagramViewModel OpenBpmnPage(bool? isCopy, Guid? id, string filePath);
    void CheckVersion(Guid reqTypeId, int orgVersion, int secVersion);
    void DeleteWorkFlowById(Guid id);
    IEnumerable<WorkFlowViewModel> GetByStaffRegister(Guid userStaffId);
    Guid GetWorkflowIdForDetails(Guid requestTypeId);
    Stream DownloadPackage(Guid id);
    string CheckForDownloadPackage(Guid id);
    void UploadPackage(PackageViewModel model, string username);
    IEnumerable<WorkFlowViewModel> GetSubProcessOfWorkFlow(Guid id);
    Guid GenerateEntityFromModel(XDocument myxml, BpmnDiagramViewModel model);
    Tuple<Guid, string> UpdateEntityFromModel(XDocument myxml, BpmnDiagramViewModel model);
    Guid CopyDiagram(Guid id, Guid? parrentId, int orginal = 0, int secondry = 0, string filePath = "", string username = "");
    IEnumerable<WorkFlowViewModel> GetActiveWorkFlows(string username);
    List<WorkflowTutorialDownloadViewModel> GetWorkflowForTutorialDownload(string username);
    List<ExternalApiInWorkFlowsViewModel> ExternalApiInWorkFlows(Guid externalApiId);
    List<SelectListItem> GetWorkFlowForms(Guid workFlowId);
    IEnumerable<WorkFlowViewModel> GetByStaffRegisterByPolicy(string username);
    Guid GetWorkFlowRequestTypeId(Guid workFlowId);
    Tuple<int, int> GetNewVersion(Workflow workflow);
    Workflow GetActiveWorkflowByRemoteId(string remoteId);
    Workflow FindById(Guid? id);
    List<Guid> GetStaffTopNRequests(Guid staffId, int n);
}