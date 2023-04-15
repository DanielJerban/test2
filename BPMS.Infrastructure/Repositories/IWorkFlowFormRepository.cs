using BPMS.Domain.Common.Dtos;
using BPMS.Domain.Common.ViewModels;
using BPMS.Domain.Entities;
using BPMS.Infrastructure.Repositories.Base;
using Microsoft.AspNetCore.Http;

namespace BPMS.Infrastructure.Repositories;

public interface IWorkFlowFormRepository : IRepository<WorkFlowForm>
{
    IEnumerable<WorkFlowFormDto> GetWorkFlowForm();
    Guid CreateNewFormOrUpdate(WorkFlowFormViewModel model, string username);
    IEnumerable<WorkFlowFormViewModel> GetAllWorkFlowForms();
    List<ExpersionViewModel> GetPropertiesFromStaticForm(Guid requestTypeId);
    List<ExpersionViewModel> GetPropertiesFromDynamicFormByConnectionId(string connectionId, string xml);
    void ValidateForm(WorkFlowFormViewModel model);
    List<ExpersionViewModel> GetPropertiesFromSingleForm(Guid formId);
    List<PropertySubformViewModel> GetPropertySubFormOfForm(Guid workFlowFormId);

    void UploadFile(List<IFormFile> files, Guid requestId, string webRootPath);
    IEnumerable<SelectListItem> GetPropertySubFormOfForm(string content);
    string CheckReadonlNotChange(string model, Guid paramRequestId, List<string> readonlis);
    IEnumerable<WorkFlowFormViewModel> GetByAccess(Guid userStaffId);
    IEnumerable<WorkFlowFormDto> GetWorkFlowFormWithVersion();
    List<PreviousFormViewModel> GetPreviousForm(string json, Flow flow);
    List<ExpersionViewModel> GetPropertiesFromDynamicForm(string xml, Guid? workflowId);
    void UpdateWorkFlowDetails(string[] formData, string formTitle);
    IEnumerable<WorkFlowFormViewModel> GetByAccessPolicy(string username);
    List<PropertySubformViewModel> GetAllPropertyOfForm(Guid workFlowFormId);
    WorkFlowFormVersionViewModel MakeVersion(int maxOrg, int maxSec);
}